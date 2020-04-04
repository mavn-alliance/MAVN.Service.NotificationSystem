using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.NotificationSystem.Client;
using MAVN.Service.NotificationSystem.Client.Models.Message;
using MAVN.Service.NotificationSystem.Domain.Models;
using MAVN.Service.NotificationSystem.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.NotificationSystem.Controllers
{
    [Route("/api/message/")]
    [ApiController]
    [Produces("application/json")]
    public class NotificationMessageController : Controller, INotificationMessageApi
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;

        public NotificationMessageController(ILogFactory logFactory, IMapper mapper, IMessageService messageService)
        {
            _mapper = mapper;
            _messageService = messageService;
            _log = logFactory.CreateLog(this);
        }

        /// <inheritdoc/>
        /// <response code="200"></response>
        [HttpPost("email")]
        [SwaggerOperation("Send email to customer")]
        [ProducesResponseType(typeof(MessageResponseModel), (int) HttpStatusCode.OK)]
        public async Task<MessageResponseModel> SendEmailAsync(SendEmailRequest model)
        {
            var emailMessage = _mapper.Map<EmailMessage>(model);

            var messageResponseContract = await _messageService.ProcessEmailAsync(emailMessage, CallType.Rest);

            return _mapper.Map<MessageResponseModel>(messageResponseContract);
        }

        /// <inheritdoc/>
        /// <response code="200"></response>
        [HttpPost("sms")]
        [SwaggerOperation("Send SMS to customer")]
        [ProducesResponseType(typeof(MessageResponseModel), (int)HttpStatusCode.OK)]
        public async Task<MessageResponseModel> SendSmsAsync(SendSmsRequest model)
        {
            var sms = _mapper.Map<Sms>(model);

            var messageResponseContract = await _messageService.ProcessSmsAsync(sms, CallType.Rest);

            return _mapper.Map<MessageResponseModel>(messageResponseContract);
        }

        /// <inheritdoc/>
        /// <response code="200"></response>
        [HttpPost("pushNotification")]
        [SwaggerOperation("Send Push Notification to customer")]
        [ProducesResponseType(typeof(MessageResponseModel), (int)HttpStatusCode.OK)]
        public async Task<MessageResponseModel> SendPushNotificationAsync(SendPushNotificationRequest model)
        {
            var pushNotification = _mapper.Map<PushNotification>(model);

            var messageResponseContract = await _messageService.ProcessPushNotificationAsync(pushNotification, CallType.Rest);

            return _mapper.Map<MessageResponseModel>(messageResponseContract);
        }
    }
}
