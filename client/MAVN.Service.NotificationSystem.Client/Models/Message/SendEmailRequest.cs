using System.Collections.Generic;

namespace MAVN.Service.NotificationSystem.Client.Models.Message
{
    /// <summary>
    /// Send email request
    /// </summary>
    public class SendEmailRequest
    {
        /// <summary>
        /// Customer id, recipient
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Subject template id
        /// </summary>
        public string SubjectTemplateId { get; set; }

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
