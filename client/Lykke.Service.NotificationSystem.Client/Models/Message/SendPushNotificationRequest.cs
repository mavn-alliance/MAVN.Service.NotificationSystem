using System.Collections.Generic;

namespace Lykke.Service.NotificationSystem.Client.Models.Message
{
    /// <summary>
    /// Send push notification request
    /// </summary>
    public class SendPushNotificationRequest
    {
        /// <summary>
        /// Customer id, recipient
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Message template id
        /// </summary>
        public string MessageTemplateId { get; set; }

        /// <summary>
        /// Represent custom payload
        /// </summary>
        public Dictionary<string, string> CustomPayload { get; set; }

        /// <summary>
        /// Template parameters with values
        /// </summary>
        public Dictionary<string, string> TemplateParameters { get; set; }

        /// <summary>
        /// ID of the service sending the message. It is recommended to use: service name + service version
        /// </summary>
        public string Source { get; set; }
    }
}
