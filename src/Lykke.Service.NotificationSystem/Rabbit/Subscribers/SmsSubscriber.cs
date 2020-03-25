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
    public class SmsSubscriber : ISmsSubscriber
    {
        private readonly ILogFactory _logFactory;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<SmsEvent> _subscriber;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public SmsSubscriber(ILogFactory logFactory, string connectionString, string exchangeName,
            IMapper mapper, IMessageService messageService, string queueName)
        {
            _logFactory = logFactory;
            _mapper = mapper;
            _messageService = messageService;

            _log = logFactory.CreateLog(this);

            _settings = RabbitMqSubscriptionSettings.ForSubscriber(connectionString, exchangeName, queueName)
                .MakeDurable();
        }

        public void Start()
        {
            _subscriber = new RabbitMqSubscriber<SmsEvent>(
                    _logFactory,
                    _settings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        _settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<SmsEvent>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        private async Task ProcessMessageAsync(SmsEvent message)
        {
            var context = new
            {
                message.CustomerId,
                message.MessageTemplateId,
                message.Source
            };

            _log.Info("Sms subscriber received message", context);

            try
            {
                await _messageService.ProcessSmsAsync(_mapper.Map<Sms>(message), CallType.RabbitMq);
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to process sms message", context);
                return;
            }

            _log.Info("Sms subscriber processed message", context);
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
