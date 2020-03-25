using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.NotificationSystem.Domain.Models;
using Lykke.Service.NotificationSystem.Domain.Services;
using Lykke.Service.NotificationSystem.Domain.Subscribers;
using Lykke.Service.NotificationSystem.SubscriberContract;

namespace Lykke.Service.NotificationSystem.Rabbit.Subscribers
{
    public class EmailMessageSubscriber : IEmailMessageSubscriber
    {
        private readonly ILogFactory _logFactory;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<EmailMessageEvent> _subscriber;
        private readonly IMessageService _messageService;
        private readonly string _queueName;
        private readonly IMapper _mapper;

        public EmailMessageSubscriber(ILogFactory logFactory, string connectionString, string exchangeName,
            IMapper mapper, IMessageService messageService, string queueName)
        {
            _logFactory = logFactory;
            _mapper = mapper;
            _messageService = messageService;
            _queueName = queueName;

            _log = logFactory.CreateLog(this);

            _settings = RabbitMqSubscriptionSettings.ForSubscriber(connectionString, exchangeName, _queueName)
                .MakeDurable();
        }

        public void Start()
        {
            _subscriber = new RabbitMqSubscriber<EmailMessageEvent>(
                    _logFactory,
                    _settings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        _settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<EmailMessageEvent>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        private async Task ProcessMessageAsync(EmailMessageEvent message)
        {
            var context = new
            {
                message.CustomerId,
                message.MessageTemplateId,
                message.Source,
                message.SubjectTemplateId
            };

            _log.Info("Email message subscriber received message", context);

            try
            {
                await _messageService.ProcessEmailAsync(_mapper.Map<EmailMessage>(message), CallType.RabbitMq);
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to process email message", context);
                return;
            }
            
            _log.Info("Email message subscriber processed message", context);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }
    }
}
