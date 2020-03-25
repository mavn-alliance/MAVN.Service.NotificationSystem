using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.Logs;
using Lykke.Service.NotificationSystem.Contract.Enums;
using Lykke.Service.NotificationSystem.Contract.MessageContracts;
using Lykke.Service.NotificationSystem.Domain.Models;
using Lykke.Service.NotificationSystem.Domain.Publishers;
using Lykke.Service.NotificationSystem.Domain.Services;
using Lykke.Service.NotificationSystem.DomainServices;
using Lykke.Service.NotificationSystemAdapter.Client;
using Moq;
using Xunit;
using CallType = Lykke.Service.NotificationSystem.Domain.Models.CallType;

namespace Lykke.Service.NotificationSystem.Tests
{
    public class MessageServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly MessageService _messageService;
        private readonly Mock<ITemplateService> _templateServiceMock = new Mock<ITemplateService>();

        private readonly Mock<INotificationSystemAdapterClient> _notificationSystemAdapterClient =
            new Mock<INotificationSystemAdapterClient>();

        private readonly Mock<IBrokerMessageEventPublisher> _sendBrokerMessageEventPublisher =
            new Mock<IBrokerMessageEventPublisher>();

        private const string IdentityNamespace = "test";
        private const string EmailKey = "test";
        private const string PhoneNumberKey = "test";
        private const string LocalizationKey = "test";

        private readonly Mock<ICreateAuditMessageEventPublisher> _createAuditMessageEventPublisher =
            new Mock<ICreateAuditMessageEventPublisher>();

        private readonly IMapper _mapper;

        public MessageServiceTests()
        {
            _mapper = Helpers.CreateAutoMapper();

            _messageService = new MessageService(EmptyLogFactory.Instance, _templateServiceMock.Object,
                _notificationSystemAdapterClient.Object, _sendBrokerMessageEventPublisher.Object, IdentityNamespace,
                EmailKey,
                PhoneNumberKey, LocalizationKey, _createAuditMessageEventPublisher.Object, _mapper);
        }

        [Fact]
        public async Task When_Process_Email_Async_Is_Executed_Expect_That_Notification_System_Adapter_Is_Called()
        {
            var data = _fixture.Build<EmailMessage>().Create();

            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<Dictionary<string, string>>().Create()));

            await _messageService.ProcessEmailAsync(data, It.IsAny<CallType>());

            _notificationSystemAdapterClient.Verify(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [Theory]
        [InlineData("original", "new")]
        [InlineData("asdsa", "sd")]
        [InlineData("string@abv.bg", "string@mail.bg")]
        public async Task When_Process_Email_Async_Is_Executed_With_Target_Email_Expect_Broker_Is_Called_With_New_Email(string originalEmail, string newEmail)
        {
            var data = _fixture.Build<EmailMessage>().Create();
            var identity = new Dictionary<string, string>()
            {
                {"test", originalEmail}
            };
            
            data.TemplateParameters.Add("target_email", newEmail);

            _notificationSystemAdapterClient
                .Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(identity);
            _templateServiceMock
                .Setup(t => t.FindTemplateContentAsync(It.IsAny<string>(), It.IsAny<Localization>()))
                .ReturnsAsync(new NotificationTemplateContent("test", Localization.Default, string.Empty));

            var passedEmail = string.Empty;
            _sendBrokerMessageEventPublisher
                .Setup(b => b.PublishAsync(It.IsAny<BrokerMessage>()))
                .Callback<BrokerMessage>(brokerData =>
                {
                    passedEmail = brokerData.Properties["Email"];
                })
                .Returns(Task.CompletedTask);

            await _messageService.ProcessEmailAsync(data, It.IsAny<CallType>());

            _sendBrokerMessageEventPublisher
                .Verify(b => b.PublishAsync(It.IsAny<BrokerMessage>()), Times.Once);

            Assert.Equal(newEmail, passedEmail);
            Assert.NotEqual(originalEmail, passedEmail);
        }

        [Theory]
        [InlineData("original")]
        [InlineData("asdsa")]
        [InlineData("string@abv.bg")]
        public async Task When_Process_Email_Async_Is_Executed_Expect_Broker_Is_Called_With_Email(string email)
        {
            var data = _fixture.Build<EmailMessage>().Create();
            var identity = new Dictionary<string, string>()
            {
                {"test", email}
            };

            _notificationSystemAdapterClient
                .Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(identity);
            _templateServiceMock
                .Setup(t => t.FindTemplateContentAsync(It.IsAny<string>(), It.IsAny<Localization>()))
                .ReturnsAsync(new NotificationTemplateContent("test", Localization.Default, string.Empty));

            var passedEmail = string.Empty;
            _sendBrokerMessageEventPublisher
                .Setup(b => b.PublishAsync(It.IsAny<BrokerMessage>()))
                .Callback<BrokerMessage>(brokerData =>
                {
                    passedEmail = brokerData.Properties["Email"];
                })
                .Returns(Task.CompletedTask);

            await _messageService.ProcessEmailAsync(data, It.IsAny<CallType>());

            _sendBrokerMessageEventPublisher
                .Verify(b => b.PublishAsync(It.IsAny<BrokerMessage>()), Times.Once);

            Assert.Equal(email, passedEmail);
        }

        [Fact]
        public async Task When_Process_Sms_Async_Is_Executed_Expect_That_Notification_System_Adapter_Is_Called()
        {
            var data = _fixture.Build<Sms>().Create();

            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<Dictionary<string, string>>().Create()));

            await _messageService.ProcessSmsAsync(data, It.IsAny<CallType>());

            _notificationSystemAdapterClient.Verify(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task When_Process_PushNotification_Async_Is_Executed_Expect_That_Notification_System_Adapter_Is_Called()
        {
            var data = _fixture.Build<PushNotification>().Create();

            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<Dictionary<string, string>>().Create()));

            await _messageService.ProcessPushNotificationAsync(data, It.IsAny<CallType>());

            _notificationSystemAdapterClient.Verify(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(2));
        }

        [Fact]
        public void When_Format_Namespace_Key_Is_Executed_Expect_That_Key_Is_Formatted_Properly()
        {
            var @namespace = "testnamespace";
            var key = "testkey";

            var result = _messageService.FormatNamespaceKey(@namespace, key);

            Assert.Equal(result, $"{@namespace}::{key}");
        }

        [Fact]
        public void When_Format_Dictionary_Keys_As_Parameters_Is_Executed_Expect_That_Key_Is_Formatted_Properly()
        {
            var data = "dummy";
            var expectedData = "${dummy}";

            var result = _messageService.TemplateFormatParameterKey(data);

            Assert.Equal(result, expectedData);
        }

        [Fact]
        public void When_Get_Namespaces_Is_Executed_Expect_That_Only_Namespaces_Keys_Are_Extracted()
        {
            var data = new NotificationTemplateContent(
                "test",
                new Localization("en", ""),
                "test content ${testnamespace::testkey}");

            var expectedResult = new List<string> {"testnamespace"};

            var result = _messageService.GetNamespaces(data);

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void When_Process_Template_Is_Executed_With_Valid_Parameters_Expect_That_Content_Is_Updated()
        {
            var contentData = "test content ${testnamespace::testkey}";
            var expectedResult = "test content ahoy";
            var parameters = new Dictionary<string, string>
            {
                {"testnamespace::testkey", "ahoy"}
            };

            var result = _messageService.ProcessTemplate(contentData, parameters);

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void When_Process_Template_Is_Executed_With_Invalid_Parameters_Expect_That_Content_Is_Not_Updated()
        {
            var contentData = "test content ${testnamespace::testkey}";
            var parameters = new Dictionary<string, string>
            {
                {"testnamespace::test", "ahoy"}
            };

            var result = _messageService.ProcessTemplate(contentData, parameters);

            Assert.Equal(result, contentData);
        }

        [Fact]
        public async Task When_Send_Email_Message_Async_Is_Executed_Expect_That_Broker_Message_Publisher_Is_Called()
        {
            _sendBrokerMessageEventPublisher.Setup(x => x.PublishAsync(It.IsAny<BrokerMessage>()))
                .Returns(Task.CompletedTask);

            await _messageService.SendEmailMessageAsync(Guid.NewGuid(), "test", "test", "test",
                new Dictionary<string, string>());

            _sendBrokerMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<BrokerMessage>()), Times.Once());
        }

        [Fact]
        public async Task When_Send_Sms_Async_Is_Executed_Expect_That_Broker_Message_Publisher_Is_Called()
        {
            _sendBrokerMessageEventPublisher.Setup(x => x.PublishAsync(It.IsAny<BrokerMessage>()))
                .Returns(Task.CompletedTask);

            await _messageService.SendSmsAsync(Guid.NewGuid(), "test", "test", new Dictionary<string, string>());

            _sendBrokerMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<BrokerMessage>()), Times.Once());
        }

        [Fact]
        public async Task When_Send_PushNotification_Async_Is_Executed_Expect_That_Broker_Message_Publisher_Is_Called()
        {
            _sendBrokerMessageEventPublisher.Setup(x => x.PublishAsync(It.IsAny<BrokerMessage>()))
                .Returns(Task.CompletedTask);

            await _messageService.SendPushNotificationAsync(Guid.NewGuid(), Guid.NewGuid(), 
                "test", "test", "test", new Dictionary<string, string>(),
                new Dictionary<string, string>());

            _sendBrokerMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<BrokerMessage>()), Times.Once());
        }

        [Fact]
        public async Task When_Get_Keys_Async_Is_Executed_Expect_That_Notification_System_Adapter_Is_Called()
        {
            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Dictionary<string, string>()));

            await _messageService.GetNamespaceParamsAsync("test", new[] {"test"});

            _notificationSystemAdapterClient.Verify(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());
        }

        [Fact]
        public async Task When_Get_Keys_Async_Is_Executed_For_Valid_Namespace_Expect_That_Parameters_Are_Extracted()
        {
            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Dictionary<string, string> {{"test", "test"}}));

            var result = await _messageService.GetNamespaceParamsAsync("test", new[] {"test"});

            Assert.Equal(result, new Dictionary<string, string> {{"test::test", "test"}});
        }

        [Fact]
        public async Task When_Get_Namespace_Params_Async_Is_Executed_Expect_That_Distinct_Namespace_Parameters_Are_Found()
        {
            var customerId = Guid.NewGuid().ToString();

            var identityParameters = new Dictionary<string, string>
            {
                { "key1", "value1" }
            };

            var templates = new[]
            {
                new NotificationTemplateContent("name1", Localization.Default, "content1 ${PersonalData::Email}"), 
            };

            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityParameters));

            var result = await _messageService.GetNamespaceParamsAsync(customerId, identityParameters, templates);
            var expectedResult = new Dictionary<string, string>(identityParameters)
            {
                {"personaldata::key1", "value1"}
            };

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void
            When_Create_Formatting_Comment_Is_Executed_And_Template_Content_Has_Unresolved_Parameters_Then_Proper_Message_Is_Returned()
        {
            var template = new NotificationTemplateContent("test", Localization.Default, "content ${PersonalData::Email} content");
            var result = _messageService.CreateFormattingComment(template);

            Assert.Contains("There were missing parameter values inside message:", result);
            Assert.Contains("PersonalData::Email", result);
        }

        [Fact]
        public void
            When_Create_Formatting_Comment_Is_Executed_And_Template_Content_Has_All_Parameters_Resolved_Then_Proper_Message_Is_Returned()
        {
            var template = new NotificationTemplateContent("test", Localization.Default, "content");
            var result = _messageService.CreateFormattingComment(template);

            Assert.Contains("There were missing parameter values inside message:", result);
        }

        [Fact]
        public void
            When_Validate_Message_Formatting_Is_Executed_And_Message_Does_Not_Have_Unresolved_Parameters_Then_Formatting_Status_And_Comment_Are_Valid()
        {
            var message = "message content";
            var createAuditMessageEvent = _fixture.Build<CreateAuditMessageEvent>()
                .With(x => x.FormattingStatus, FormattingStatus.Success)
                .With(x => x.FormattingComment, string.Empty).Create();

            _messageService.ValidateMessageFormatting(message, createAuditMessageEvent);

            Assert.Equal(string.Empty, createAuditMessageEvent.FormattingComment);
            Assert.Equal(FormattingStatus.Success, createAuditMessageEvent.FormattingStatus);
        }

        [Fact]
        public void
            When_Validate_Message_Formatting_Is_Executed_And_Message_Have_Unresolved_Parameters_Then_Formatting_Status_And_Comment_Are_Valid()
        {
            var message = "message ${PersonalData:Email} content";
            var createAuditMessageEvent = _fixture.Build<CreateAuditMessageEvent>()
                .With(x => x.FormattingStatus, FormattingStatus.Success)
                .With(x => x.FormattingComment, string.Empty).Create();

            _messageService.ValidateMessageFormatting(message, createAuditMessageEvent);

            Assert.NotEmpty(createAuditMessageEvent.FormattingComment);
            Assert.Equal(FormattingStatus.ValueNotFound, createAuditMessageEvent.FormattingStatus);
        }

        [Fact]
        public void
            When_Get_Localization_Is_Executed_And_No_Localization_Is_Found_Then_Default_Localization_Is_Returned()
        {
            var identityParameters = new Dictionary<string, string>();

            var result = _messageService.GetLocalization(identityParameters);

            var expectedResult = new Localization("en", "");

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void
            When_Get_Localization_Is_Executed_And_Localization_Is_Found_Then_Found_Localization_Is_Returned()
        {
            var identityParameters = new Dictionary<string, string>
            {
                { "test::test", "srb" }
            };

            var result = _messageService.GetLocalization(identityParameters);

            var expectedResult = new Localization("srb", "");

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public async Task When_Publish_Create_Audit_Message_Event_Is_Executed_For_Sms_Then_Proper_Publisher_Is_Called()
        {
            var callType = CallType.Rest;
            var messageId = Guid.NewGuid();
            var message = "content";
            var sms = new Sms
            {
                CustomerId = Guid.NewGuid().ToString(),
                TemplateParameters = new Dictionary<string, string>(),
                MessageTemplateId = Guid.NewGuid().ToString(),
                Source = string.Empty
            };

            await _messageService.PublishCreateAuditMessageEvent(callType, messageId, sms, message);

            _createAuditMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<CreateAuditMessageEvent>()), Times.Once());
        }

        [Fact]
        public async Task When_Publish_Create_Audit_Message_Event_Is_Executed_For_PushNotification_Then_Proper_Publisher_Is_Called()
        {
            var callType = CallType.Rest;
            var messageId = Guid.NewGuid();
            var messageGroupId = Guid.NewGuid();
            var message = "content";
            var pushNotification = new PushNotification
            {
                CustomerId = Guid.NewGuid().ToString(),
                TemplateParameters = new Dictionary<string, string>(),
                MessageTemplateId = Guid.NewGuid().ToString(),
                Source = string.Empty
            };

            await _messageService.PublishCreateAuditMessageEvent(callType, messageId, messageGroupId, pushNotification,
                message);

            _createAuditMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<CreateAuditMessageEvent>()), Times.Once());
        }

        [Fact]
        public async Task When_Publish_Create_Audit_Message_Event_Is_Executed_For_Email_Then_Proper_Publisher_Is_Called()
        {
            var callType = CallType.Rest;
            var messageId = Guid.NewGuid();
            var message = "content";
            var email = new EmailMessage
            {
                CustomerId = Guid.NewGuid().ToString(),
                TemplateParameters = new Dictionary<string, string>(),
                MessageTemplateId = Guid.NewGuid().ToString(),
                Source = string.Empty,
                SubjectTemplateId = Guid.NewGuid().ToString()
            };

            await _messageService.PublishCreateAuditMessageEvent(callType, messageId, email, message);

            _createAuditMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<CreateAuditMessageEvent>()), Times.Once());
        }

        [Fact]
        public async Task When_ProcessPushNotificationAsync_IsExecutedWithNoPushIds_Expect_NothingPublished()
        {
            //Arange
            var messageTemplateId = Guid.NewGuid().ToString();
            var customerId = Guid.NewGuid().ToString();
            var pushNotification = new PushNotification
            {
                CustomerId = customerId,
                TemplateParameters = new Dictionary<string, string>(),
                MessageTemplateId = messageTemplateId,
                Source = "Test",
                CustomPayload = new Dictionary<string, string>()
            };

            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync("test", customerId))
                .Returns(Task.FromResult(new Dictionary<string, string>()));
            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync("pushnotifications", customerId))
                .Returns(Task.FromResult(new Dictionary<string, string>()));

            //Act
            await _messageService.ProcessPushNotificationAsync(pushNotification, CallType.Rest);

            //Assert
            _createAuditMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<CreateAuditMessageEvent>()), Times.Never);
            _sendBrokerMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<BrokerMessage>()), Times.Never);
        }

        [Fact]
        public async Task When_ProcessPushNotificationAsync_IsExecuted_Expect_MessageParametersExtracted()
        {
            //Arange
            var messageTemplateId = Guid.NewGuid().ToString();
            var customerId = Guid.NewGuid().ToString();
            var transactionId = Guid.NewGuid().ToString();
            var pushNotification = new PushNotification
            {
                CustomerId = customerId,
                TemplateParameters = new Dictionary<string, string>()
                {
                    { "transactionId", transactionId }
                },
                MessageTemplateId = messageTemplateId,
                Source = "Test",
                CustomPayload = new Dictionary<string, string>()
            };

            var content =
                "@@@{\"deeplink\":\"/transactions?id=${transactionId}\",\"image\":\"biglogo.png\"}@@@Hello ${PersonalData::FirstName},\nGood to know you.\nBye";

            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync("test", customerId))
                .Returns(Task.FromResult(new Dictionary<string, string>()));
            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync("personaldata", customerId))
                .Returns(Task.FromResult(new Dictionary<string, string>()));
            _notificationSystemAdapterClient.Setup(x => x.Api.GetKeysAsync("pushnotifications", customerId))
                .Returns(Task.FromResult(new Dictionary<string, string>()
                {
                    {"PushRegistrationIds", $"[\"{Guid.NewGuid().ToString()}\"]"}
                }));
            _templateServiceMock.Setup(x => x.FindTemplateContentAsync(messageTemplateId, Localization.From("en")))
                .Returns(Task.FromResult(new NotificationTemplateContent(messageTemplateId, Localization.From("en"), content)));

            //Act
            await _messageService.ProcessPushNotificationAsync(pushNotification, CallType.Rest);

            //Assert
            _createAuditMessageEventPublisher.Verify(x => x.PublishAsync(It.IsAny<CreateAuditMessageEvent>()), Times.Once);

            var deeplink = $"/transactions?id={transactionId}";
            var image = "biglogo.png";
            _sendBrokerMessageEventPublisher.Verify(x => x.PublishAsync(It.Is<BrokerMessage>(bm => bm.MessageParameters["deeplink"] == deeplink)), Times.Once);
            _sendBrokerMessageEventPublisher.Verify(x => x.PublishAsync(It.Is<BrokerMessage>(bm => bm.MessageParameters["image"] == image)), Times.Once);
        }
    }
}
