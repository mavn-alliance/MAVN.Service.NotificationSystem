using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.NotificationSystemAdapter.Client;

namespace Lykke.Service.NotificationSystem.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public NotificationSystemSettings NotificationSystemService { get; set; }

        public NotificationSystemAdapterServiceClientSettings NotificationSystemAdapterServiceClient { get; set; }

        public RabbitMqSettings Rabbit { get; set; }
    }
}
