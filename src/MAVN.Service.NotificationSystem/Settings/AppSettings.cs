using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.NotificationSystemAdapter.Client;

namespace MAVN.Service.NotificationSystem.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public NotificationSystemSettings NotificationSystemService { get; set; }

        public NotificationSystemAdapterServiceClientSettings NotificationSystemAdapterServiceClient { get; set; }

        public RabbitMqSettings Rabbit { get; set; }
    }
}
