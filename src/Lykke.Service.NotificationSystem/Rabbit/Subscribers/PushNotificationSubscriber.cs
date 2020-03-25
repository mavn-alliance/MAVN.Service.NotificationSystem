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
    public class PushNotificationSubscriber : IPushNotificationsSubscriber
    {
        private readonly ILogFactory _logFactory;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<PushNotificationEvent> _subscriber;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public PushNotificationSubscriber(ILogFactory logFactory, string connectionString, string exchangeName,
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
            _subscriber = new RabbitMqSubscriber<PushNotificationEvent>(
                    _logFactory,
                    _settings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        _settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<PushNotificationEvent>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        private async Task ProcessMessageAsync(PushNotificationEvent message)
        {
            var context = new {message.CustomerId, message.MessageTemplateId, message.Source};

            _log.Info("Push notification subscriber received message", context);

            try
            {
                await _messageService.ProcessPushNotificationAsync(_mapper.Map<PushNotification>(message), CallType.RabbitMq);
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to process push notification message", context);
                return;
            }
            
            _log.Info("Push notification subscriber processed message", context);
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
