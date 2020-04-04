using System.Collections.Generic;

namespace MAVN.Service.NotificationSystem.SubscriberContract
{
    /// <summary>
    /// Represents a push notification event
    /// </summary>
    public class PushNotificationEvent
    {
        /// <summary>
        /// Represents the Id of the customer
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Represents the id of the template that is used for message
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
