using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.NotificationSystem.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string EmailMessageSubscriberQueueName { get; set; }

        public string PushNotificationSubscriberQueueName { get; set; }

        public string SmsSubscriberQueueName { get; set; }
    }
}
