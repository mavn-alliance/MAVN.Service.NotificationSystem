using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NotificationSystemAdapter.Client;
using MAVN.Service.NotificationSystem.Contract.Enums;
using MAVN.Service.NotificationSystem.Contract.MessageContracts;
using MAVN.Service.NotificationSystem.Domain.Enums;
using MAVN.Service.NotificationSystem.Domain.Models;
using MAVN.Service.NotificationSystem.Domain.Publishers;
using MAVN.Service.NotificationSystem.Domain.Services;
using MoreLinq;
using Newtonsoft.Json;
using CallType = MAVN.Service.NotificationSystem.Domain.Models.CallType;

namespace MAVN.Service.NotificationSystem.DomainServices
{
    public class MessageService : IMessageService
    {
        private const string DefaultLocalization = "en";
        private const string EmailParamKey = "target_email";
        private const string PhoneNumberParamKey = "target_phonenumber";
        private readonly string _messageParametersRegexPattern = "^@@@(?<MessageParameters>.*?)@@@(?<MessageText>.*?)$";
        private readonly ILog _log;
        private readonly ITemplateService _templateService;
        private readonly INotificationSystemAdapterClient _notificationSystemAdapterClient;
        private readonly IBrokerMessageEventPublisher _brokerMessageEventPublisher;
        private readonly string _identityNamespace;
        private readonly string _pushNotificationsNamespace = "pushnotifications";
        private readonly string _emailKey;
        private readonly string _phoneNumberKey;
        private readonly string _localizationKey;
        private readonly string _pushRegistrationIdsKey = "PushRegistrationIds";
        private readonly ICreateAuditMessageEventPublisher _createAuditMessageEventPublisher;
        private readonly IMapper _mapper;

        public MessageService(ILogFactory logFactory, ITemplateService templateService,
            INotificationSystemAdapterClient notificationSystemAdapterClient,
            IBrokerMessageEventPublisher brokerMessageEventPublisher, string identityNamespace, string emailKey,
            string phoneNumberKey, string localizationKey,
            ICreateAuditMessageEventPublisher createAuditMessageEventPublisher, IMapper mapper)
        {
            _templateService = templateService;
            _notificationSystemAdapterClient = notificationSystemAdapterClient;
            _brokerMessageEventPublisher = brokerMessageEventPublisher;
            _identityNamespace = identityNamespace.ToLower();
            _emailKey = emailKey;
            _phoneNumberKey = phoneNumberKey;
            _localizationKey = localizationKey;
            _createAuditMessageEventPublisher = createAuditMessageEventPublisher;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        public async Task<MessageResponseContract> ProcessEmailAsync(EmailMessage emailMessage, CallType callType)
        {
            var messageResponse = new MessageResponseContract
            {
                Status = ResponseStatus.Success
            };

            if (string.IsNullOrEmpty(emailMessage.CustomerId))
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find customer id");

                return messageResponse;
            }

            //Check if we know where the message is from
            if (string.IsNullOrEmpty(emailMessage.Source))
            {
                ProcessErrorMessageResponse(messageResponse, "Message source field is not set",
                    emailMessage.CustomerId);

                return messageResponse;
            }

            if (string.IsNullOrEmpty(emailMessage.SubjectTemplateId))
            {
                ProcessErrorMessageResponse(messageResponse, "Message subject template id is not set",
                    new {emailMessage.CustomerId, emailMessage.Source});

                return messageResponse;
            }

            if (string.IsNullOrEmpty(emailMessage.MessageTemplateId))
            {
                ProcessErrorMessageResponse(messageResponse, "Message template id is not set",
                    new { emailMessage.CustomerId, emailMessage.Source });

                return messageResponse;
            }

            var identityParameters = await GetIdentityParameters(emailMessage.CustomerId);

            // Temporary work-around to send notifications for non-customers
            string email;
            if (emailMessage.TemplateParameters.ContainsKey(EmailParamKey) &&
                !string.IsNullOrEmpty(emailMessage.TemplateParameters[EmailParamKey]))
            {
                email = emailMessage.TemplateParameters[EmailParamKey];
            }
            // Get customer's email
            else if (!identityParameters.TryGetValue(GetFormattedEmailKey(), out email) || string.IsNullOrEmpty(email))
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find customer's email",
                    new { emailMessage.CustomerId, Key = _emailKey, emailMessage.Source });

                return messageResponse;
            }

            var localization = GetLocalization(identityParameters);

            //Find subject template
            var subjectTemplate =
                await _templateService.FindTemplateContentAsync(emailMessage.SubjectTemplateId, localization);
            if (subjectTemplate == null)
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find subject template",
                    new { emailMessage.CustomerId, emailMessage.SubjectTemplateId, emailMessage.Source });

                return messageResponse;
            }

            //Find message template
            var messageTemplate =
                await _templateService.FindTemplateContentAsync(emailMessage.MessageTemplateId, localization);
            if (messageTemplate == null)
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find message template",
                    new { emailMessage.CustomerId, emailMessage.MessageTemplateId, emailMessage.Source });

                return messageResponse;
            }

            // Join namespace parameters and custom parameters
            var templateParamValues = await GetNamespaceParamsAsync(emailMessage.CustomerId,
                identityParameters, subjectTemplate, messageTemplate);
            emailMessage.TemplateParameters.ForEach(tp => templateParamValues.Add(tp.Key, tp.Value));

            // Process subject and message templates
            var fullSubject = ProcessTemplate(subjectTemplate.Content, templateParamValues);
            var fullMessage = ProcessTemplate(messageTemplate.Content, templateParamValues);

            var subjectToSend = ProcessMessageParameters(messageResponse, fullSubject, out var subjectParameters);
            var messageToSend = ProcessMessageParameters(messageResponse, fullMessage, out var messageParameters);
            
            var messageId = Guid.NewGuid();

            // Publish audit event
            await PublishCreateAuditMessageEvent(callType, messageId, emailMessage, messageToSend);

            //Prepare email for broker and send it
            await SendEmailMessageAsync(messageId, email, subjectToSend, messageToSend, messageParameters);

            _log.Info("Email sent",
                new {emailMessage.CustomerId, emailMessage.SubjectTemplateId, emailMessage.MessageTemplateId});

            messageResponse.MessageIds.Add(messageId.ToString());

            return messageResponse;
        }

        public async Task<MessageResponseContract> ProcessSmsAsync(Sms sms, CallType callType)
        {
            var messageResponse = new MessageResponseContract
            {
                Status = ResponseStatus.Success
            };

            if (string.IsNullOrEmpty(sms.CustomerId))
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find customer id");

                return messageResponse;
            }

            //Check if we know where the message is from
            if (string.IsNullOrEmpty(sms.Source))
            {
                ProcessErrorMessageResponse(messageResponse, "Message source field is not set", sms.CustomerId);

                return messageResponse;
            }

            if (string.IsNullOrEmpty(sms.MessageTemplateId))
            {
                ProcessErrorMessageResponse(messageResponse, "Message template id is not set",
                    new {sms.CustomerId, sms.Source});

                return messageResponse;
            }

            //Get customer personal data - this is where localization will come from
            var identityParameters = await GetIdentityParameters(sms.CustomerId);

            // Temporary work-around to send notifications for non-customers
            string phoneNumber;
            if (sms.TemplateParameters.ContainsKey(PhoneNumberParamKey) &&
                !string.IsNullOrEmpty(sms.TemplateParameters[PhoneNumberParamKey]))
            {
                phoneNumber = sms.TemplateParameters[PhoneNumberParamKey];
            }
            // Get customer's phone number
            else if (!identityParameters.TryGetValue(GetFormattedPhoneNumberKey(), out phoneNumber) ||
                     string.IsNullOrEmpty(phoneNumber))
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find customer's phone number",
                    new {sms.CustomerId, Key = _phoneNumberKey, sms.Source});

                return messageResponse;
            }

            var localization = GetLocalization(identityParameters);

            //Find message template
            var messageTemplate =
                await _templateService.FindTemplateContentAsync(sms.MessageTemplateId, localization);
            if (messageTemplate == null)
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find message template",
                    new { sms.CustomerId, sms.MessageTemplateId, sms.Source });

                return messageResponse;
            }

            // Join namespace parameters and custom parameters
            var templateParamValues = await GetNamespaceParamsAsync(sms.CustomerId,
                identityParameters, messageTemplate);
            sms.TemplateParameters.ForEach(tp => templateParamValues.Add(tp.Key, tp.Value));

            // Process subject and message templates
            var fullMessage = ProcessTemplate(messageTemplate.Content, templateParamValues);

            var messageToSend = ProcessMessageParameters(messageResponse, fullMessage, out var messageParameters);

            var messageId = Guid.NewGuid();

            // Publish audit event
            await PublishCreateAuditMessageEvent(callType, messageId, sms, messageToSend);

            //Prepare email for broker and send it
            await SendSmsAsync(messageId, phoneNumber, messageToSend, messageParameters);

            _log.Info("SMS sent",
                new {sms.CustomerId, sms.MessageTemplateId});

            messageResponse.MessageIds.Add(messageId.ToString());

            return messageResponse;
        }

        public async Task<MessageResponseContract> ProcessPushNotificationAsync(PushNotification pushNotification, CallType callType)
        {
            var messageResponse = new MessageResponseContract
            {
                Status = ResponseStatus.Success
            };

            if (string.IsNullOrEmpty(pushNotification.CustomerId))
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find customer id");

                return messageResponse;
            }

            //Check if we know where the message is from
            if (string.IsNullOrEmpty(pushNotification.Source))
            {
                ProcessErrorMessageResponse(messageResponse, "Message source field is not set", pushNotification.CustomerId);

                return messageResponse;
            }

            if (string.IsNullOrEmpty(pushNotification.MessageTemplateId))
            {
                ProcessErrorMessageResponse(messageResponse, "Message template id is not set",
                    new {pushNotification.CustomerId, pushNotification.Source});

                return messageResponse;
            }

            //Get customer personal data - this is where localization will come from
            var identityParameters = await GetIdentityParameters(pushNotification.CustomerId);

            var pushNotificationParameters =
                await GetNamespaceParamsAsync(pushNotification.CustomerId, new[] {_pushNotificationsNamespace});
            
            // Get push registration ids
            if (!pushNotificationParameters.TryGetValue(GetFormattedPushRegistrationIds(), out var pushRegistrationIdsString) ||
                string.IsNullOrEmpty(pushRegistrationIdsString))
            {
                ProcessInfoMessageResponse(messageResponse, "Could not find any customer push registration ids",
                    new { pushNotification.CustomerId, Key = _pushRegistrationIdsKey, pushNotification.Source });

                return messageResponse;
            }

            var localization = GetLocalization(identityParameters);

            //Find message template
            var messageTemplate =
                await _templateService.FindTemplateContentAsync(pushNotification.MessageTemplateId, localization);
            if (messageTemplate == null)
            {
                ProcessErrorMessageResponse(messageResponse, "Could not find message template",
                    new { pushNotification.CustomerId, pushNotification.MessageTemplateId, pushNotification.Source });

                return messageResponse;
            }

            // Join namespace parameters and custom parameters
            var templateParamValues = await GetNamespaceParamsAsync(pushNotification.CustomerId,
                identityParameters, messageTemplate);
            pushNotification.TemplateParameters.ForEach(tp => templateParamValues.Add(tp.Key, tp.Value));

            // Process subject and message templates
            var fullMessage = ProcessTemplate(messageTemplate.Content, templateParamValues);

            var messageToSend = ProcessMessageParameters(messageResponse, fullMessage, out var messageParameters);

            var pushRegistrationIds = JsonConvert.DeserializeObject<List<string>>(pushRegistrationIdsString);

            if (!pushRegistrationIds.Any())
            {
                ProcessInfoMessageResponse(messageResponse, "Could not find any customer push registration ids",
                    new
                    {
                        pushNotification.CustomerId,
                        Key = _pushRegistrationIdsKey,
                        pushNotification.Source,
                        pushNotification.MessageTemplateId
                    });

                return messageResponse;
            }

            var messageGroupId = Guid.NewGuid();

            foreach (var pushRegistrationId in pushRegistrationIds)
            {
                var messageId = Guid.NewGuid();

                // Publish audit event
                await PublishCreateAuditMessageEvent(callType, messageId, messageGroupId, pushNotification,
                    messageToSend);

                //Prepare email for broker and send it
                await SendPushNotificationAsync(messageId, messageGroupId, pushRegistrationId, 
                    pushNotification.CustomerId, messageToSend, pushNotification.CustomPayload,
                    messageParameters);

                messageResponse.MessageIds.Add(messageId.ToString());
            }

            _log.Info("Push notifications sent",
                new {pushNotification.CustomerId, pushNotification.MessageTemplateId});

            return messageResponse;
        }

        private void ProcessErrorMessageResponse(MessageResponseContract messageResponse, string message, object context = null)
        {
            _log.Error(null, message, context);
            messageResponse.ErrorDescription = message;
            messageResponse.Status = ResponseStatus.Error;
        }

        private void ProcessInfoMessageResponse(MessageResponseContract messageResponse, string message, object context = null)
        {
            _log.Info(message, context);
            messageResponse.ErrorDescription = message;
            messageResponse.Status = ResponseStatus.Error;
        }

        private string ProcessMessageParameters(MessageResponseContract messageResponse, string fullMessage,
            out Dictionary<string, string> messageParameters)
        {
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);
            var match = regex.Match(fullMessage);

            if (!match.Success)
            {
                messageParameters = new Dictionary<string, string>();
                return fullMessage;
            }

            messageParameters = new Dictionary<string, string>();

            // We need to check that message parameters are defined at the beginning of template 
            try
            {
                messageParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(match.Groups[1].Value);
            }
            catch (Exception)
            {
                ProcessErrorMessageResponse(messageResponse,
                    "Message parameters are not in proper (JSON) format or they are not defined at the beginning of message template and thus could not be extracted",
                    new {fullMessage});
            }

            var message = match.Groups[2].Value;
            return message;
        }

        internal async Task PublishCreateAuditMessageEvent(CallType callType, Guid messageId, EmailMessage emailMessage,
            string message)
        {
            var createAuditMessageEvent = _mapper.Map<CreateAuditMessageEvent>(emailMessage);

            createAuditMessageEvent.CallType = _mapper.Map<Contract.Enums.CallType>(callType);
            createAuditMessageEvent.MessageId = messageId.ToString();

            ValidateMessageFormatting(message, createAuditMessageEvent);

            await _createAuditMessageEventPublisher.PublishAsync(createAuditMessageEvent);
        }

        internal async Task PublishCreateAuditMessageEvent(CallType callType, Guid messageId, Guid messageGroupId,
            PushNotification pushNotification, string message)
        {
            var createAuditMessageEvent = _mapper.Map<CreateAuditMessageEvent>(pushNotification);

            createAuditMessageEvent.CallType = _mapper.Map<Contract.Enums.CallType>(callType);
            createAuditMessageEvent.MessageId = messageId.ToString();
            createAuditMessageEvent.MessageGroupId = messageGroupId.ToString();

            ValidateMessageFormatting(message, createAuditMessageEvent);

            await _createAuditMessageEventPublisher.PublishAsync(createAuditMessageEvent);
        }

        internal void ValidateMessageFormatting(string message, CreateAuditMessageEvent createAuditMessageEvent)
        {
            // Check if there are any template parameters that are not changed
            // Easiest way is via new NotificationTemplateInfo
            // No need for specific template name and localization
            // We just need to check if there are keys :)
            var content = new NotificationTemplateContent("", Localization.Default, message);

            if (!content.Keys.Any())
                return;

            createAuditMessageEvent.FormattingStatus = FormattingStatus.ValueNotFound;
            createAuditMessageEvent.FormattingComment = CreateFormattingComment(content);
        }

        internal string CreateFormattingComment(NotificationTemplateContent content)
        {
            var sb = new StringBuilder();

            sb.AppendLine("There were missing parameter values inside message:");

            content.Keys.ForEach(x => sb.AppendLine(x.ToString()));

            return sb.ToString();
        }

        internal async Task PublishCreateAuditMessageEvent(CallType callType, Guid messageId, Sms sms, string message)
        {
            var createAuditMessageEvent = _mapper.Map<CreateAuditMessageEvent>(sms);

            createAuditMessageEvent.CallType = _mapper.Map<Contract.Enums.CallType>(callType);
            createAuditMessageEvent.MessageId = messageId.ToString();

            ValidateMessageFormatting(message, createAuditMessageEvent);

            await _createAuditMessageEventPublisher.PublishAsync(createAuditMessageEvent);
        }

        internal Localization GetLocalization(Dictionary<string, string> identityParameters)
        {
            if (!identityParameters.TryGetValue(FormatNamespaceKey(_identityNamespace, _localizationKey),
                out var localizationValue))
                localizationValue = DefaultLocalization; // default localization

            return Localization.From(localizationValue);
        }

        internal async Task<Dictionary<string, string>> GetNamespaceParamsAsync(string customerId,
            Dictionary<string, string> identityParams, params NotificationTemplateContent[] templates)
        {
            var templateNamespaces = templates.SelectMany(GetNamespaces)
                .Where(s => s != _identityNamespace)
                .Distinct();

            var namespaceParams = await GetNamespaceParamsAsync(customerId, templateNamespaces);
            identityParams.ForEach(ip => namespaceParams.Add(ip.Key, ip.Value));

            return namespaceParams;
        }

        private async Task<Dictionary<string, string>> GetIdentityParameters(string customerId)
        {
            return await GetNamespaceParamsAsync(customerId, new[] {_identityNamespace});
        }

        internal string FormatNamespaceKey(string @namespace, string key) => $"{@namespace}::{key}";

        internal string GetFormattedEmailKey() => FormatNamespaceKey(_identityNamespace, _emailKey);

        internal string GetFormattedPhoneNumberKey() => FormatNamespaceKey(_identityNamespace, _phoneNumberKey);

        internal string GetFormattedPushRegistrationIds() => FormatNamespaceKey(_pushNotificationsNamespace, _pushRegistrationIdsKey);

        internal string TemplateFormatParameterKey(string parameterKey) => "${" + parameterKey + "}";

        internal async Task<Dictionary<string, string>> GetNamespaceParamsAsync(string customerId,
            IEnumerable<string> templateNamespaces)
        {
            var templateParameters = new Dictionary<string, string>();

            // Get keys for all namespaces
            foreach (var templateNamespace in templateNamespaces)
            {
                var namespaceKeyValues =
                    await _notificationSystemAdapterClient.Api.GetKeysAsync(templateNamespace, customerId);

                foreach (var (key, value) in namespaceKeyValues)
                {
                    templateParameters.Add(FormatNamespaceKey(templateNamespace, key), value);
                }
            }

            return templateParameters;
        }

        internal List<string> GetNamespaces(NotificationTemplateContent templateContent) =>
            templateContent.Keys
                .Select(x => x.Namespace.ToLower())
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList();

        internal async Task SendEmailMessageAsync(Guid messageId, string customerEmail, string subject, string message,
            Dictionary<string,string> messageParameters)
        {
            var brokerMessage = new BrokerMessage
            {
                MessageId = messageId,
                Channel = Channel.Email,
                Properties = new Dictionary<string, string>
                {
                    {"Email", customerEmail},
                    {"Subject", subject},
                    {"Body", message}
                },
                MessageParameters = messageParameters
            };

            await _brokerMessageEventPublisher.PublishAsync(brokerMessage);
        }

        internal async Task SendSmsAsync(Guid messageId, string phoneNumber, string message, Dictionary<string, string> messageParameters)
        {
            var brokerMessage = new BrokerMessage
            {
                MessageId = messageId,
                Channel = Channel.Sms,
                Properties = new Dictionary<string, string>
                {
                    {"PhoneNumber", phoneNumber},
                    {"Message", message}
                },
                MessageParameters = messageParameters
            };

            await _brokerMessageEventPublisher.PublishAsync(brokerMessage);
        }

        internal async Task SendPushNotificationAsync(Guid messageId, Guid messageGroupId, string pushRegistrationId,
            string customerId, string message, Dictionary<string, string> customPayload, 
            Dictionary<string, string> messageParameters)
        {
            var brokerMessage = new BrokerMessage
            {
                MessageId = messageId,
                Channel = Channel.PushNotification,
                Properties = new Dictionary<string, string>
                {
                    {"PushRegistrationId", pushRegistrationId},
                    {"MessageGroupId", messageGroupId.ToString() },
                    {"CustomerId", customerId },
                    {"Message", message},
                    {"CustomPayload", JsonConvert.SerializeObject(customPayload)},
                },
                MessageParameters = messageParameters
            };

            await _brokerMessageEventPublisher.PublishAsync(brokerMessage);
        }

        internal string ProcessTemplate(string content, Dictionary<string, string> templateParameters)
        {
            foreach (var (key, value) in templateParameters)
            {
                content = content.Replace(TemplateFormatParameterKey(key), value, StringComparison.InvariantCultureIgnoreCase);
            }

            return content;
        }
    }
}
