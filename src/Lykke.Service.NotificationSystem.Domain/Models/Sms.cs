using System.Collections.Generic;

namespace Lykke.Service.NotificationSystem.Domain.Models
{
    public class Sms
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
        /// Template parameters with values
        /// </summary>
        public Dictionary<string, string> TemplateParameters { get; set; }

        /// <summary>
        /// ID of the service sending the message. It is recommended to use: service name + service version
        /// </summary>
        public string Source { get; set; }
    }
}
