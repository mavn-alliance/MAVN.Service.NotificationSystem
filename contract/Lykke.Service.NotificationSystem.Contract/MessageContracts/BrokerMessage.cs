using System;
using System.Collections.Generic;
using Lykke.Service.NotificationSystem.Contract.Enums;

namespace Lykke.Service.NotificationSystem.Contract.MessageContracts
{
    /// <summary>
    /// Represents a message being propagated to brokers
    /// </summary>
    public class BrokerMessage
    {
        /// <summary>
        /// Represents the Id of the message being propagated
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// Represents the chanel through wich the message should be sent
        /// </summary>
        public Channel Channel { get; set; }

        /// <summary>
        /// Represents the specific channel configurations
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Represents the custom message parameters
        /// </summary>
        public Dictionary<string, string> MessageParameters { get; set; }
    }
}
