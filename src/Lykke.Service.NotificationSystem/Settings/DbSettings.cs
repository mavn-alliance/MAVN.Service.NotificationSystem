using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.NotificationSystem.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        public string MsSqlDataConnString { get; set; }

        [AzureTableCheck]
        public string TemplateConnString { get; set; }
    }
}
