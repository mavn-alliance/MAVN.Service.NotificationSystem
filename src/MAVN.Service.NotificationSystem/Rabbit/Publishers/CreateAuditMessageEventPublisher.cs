using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.NotificationSystem.Contract.MessageContracts;
using MAVN.Service.NotificationSystem.Domain.Publishers;

namespace MAVN.Service.NotificationSystem.Rabbit.Publishers
{
    public class CreateAuditMessageEventPublisher : ICreateAuditMessageEventPublisher
    {
        private readonly ILogFactory _logFactory;
        private RabbitMqPublisher<CreateAuditMessageEvent> _publisher;
        private readonly string _connectionString;

        public CreateAuditMessageEventPublisher(ILogFactory logFactory, string connectionString)
        {
            _logFactory = logFactory;
            _connectionString = connectionString;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_connectionString, "lykke.notificationsystem.createauditmessage")
                .MakeDurable();

            _publisher = new RabbitMqPublisher<CreateAuditMessageEvent>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<CreateAuditMessageEvent>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .PublishSynchronously()
                .Start();
        }

        public void Dispose()
        {
            Stop();
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public async Task PublishAsync(CreateAuditMessageEvent message)
        {
            await _publisher.ProduceAsync(message);
        }
    }
}
