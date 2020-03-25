using JetBrains.Annotations;

namespace Lykke.Service.NotificationSystem.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class NotificationSystemSettings
    {
        public DbSettings Db { get; set; }

        public CommonSenderSettings CommonSender { get; set; }

        public EmailSenderSettings EmailSender { get; set; }

        public SmsSenderSettings SmsSender { get; set; }
    }
}
