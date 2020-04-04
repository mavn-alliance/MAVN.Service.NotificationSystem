using Lykke.HttpClientGenerator;

namespace MAVN.Service.NotificationSystem.Client
{
    /// <summary>
    /// NotificationSystem API aggregating interface.
    /// </summary>
    public class NotificationSystemClient : INotificationSystemClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to NotificationSystem Api.</summary>
        public INotificationTemplateApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public NotificationSystemClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<INotificationTemplateApi>();
        }
    }
}
