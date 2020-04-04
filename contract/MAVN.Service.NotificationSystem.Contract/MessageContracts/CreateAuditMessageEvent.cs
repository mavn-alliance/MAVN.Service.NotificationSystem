using System;
using MAVN.Service.NotificationSystem.Contract.Enums;

namespace MAVN.Service.NotificationSystem.Contract.MessageContracts
{
    /// <summary>
    /// Represents audit message creation event
    /// </summary>
    public class CreateAuditMessageEvent
    {
        /// <summary>
        /// Represents the date of creation
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Represents the id of a message that is audited
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Represents the id of the message group that is audited
        /// </summary>
        public string MessageGroupId { get; set; }

        /// <summary>
        /// Represents type of the message being audited (channel through which message came)
        /// <see cref="Channel"/>
        /// </summary>
        public Channel MessageType { get; set; }

        /// <summary>
        /// Represents and id of the customer that is receiving message
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Represents id of the template that is used as subject (when email is sent)
        /// </summary>
        public string SubjectTemplateId { get; set; }

        /// <summary>
        /// Represents id of the template that is used as message body
        /// </summary>
        public string MessageTemplateId { get; set; }

        /// <summary>
        /// Represents id of the message source (RabbitMq name, API name)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Represents type of call <see cref="CallType"/>
        /// </summary>
        public CallType CallType { get; set; }

        /// <summary>
        /// Represents the formatting status of the message <see cref="FormattingStatus"/>
        /// </summary>
        public FormattingStatus FormattingStatus { get; set; }

        /// <summary>
        /// Represents additional comment with details (list of keys, ...) in case formatting failed
        /// </summary>
        public string FormattingComment { get; set; }
    }
}
