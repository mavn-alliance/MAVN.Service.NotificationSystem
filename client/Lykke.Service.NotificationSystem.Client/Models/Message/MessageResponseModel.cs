using System.Collections.Generic;
using Lykke.Service.NotificationSystem.Client.Enums;

namespace Lykke.Service.NotificationSystem.Client.Models.Message
{
    /// <summary>
    /// Represents a response model for any type of message sent
    /// </summary>
    public class MessageResponseModel
    {
        /// <summary>
        /// Whether the message sending succeeded or failed
        /// </summary>
        public ResponseStatus Status { get; set; }

        /// <summary>
        /// If successful, the message ids of the sent messages
        /// </summary>
        public List<string> MessageIds { get; set; }

        /// <summary>
        /// If not successful, the error description
        /// </summary>
        public string ErrorDescription { get; set; }
    }
}
