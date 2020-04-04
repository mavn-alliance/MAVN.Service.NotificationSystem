using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.NotificationSystem.Client 
{
    /// <summary>
    /// NotificationSystem client settings.
    /// </summary>
    public class NotificationSystemServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
