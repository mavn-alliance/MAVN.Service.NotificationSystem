using System.Collections.Generic;
using MAVN.Service.NotificationSystem.Domain.Enums;

namespace MAVN.Service.NotificationSystem.Domain.Models
{
    public class MessageResponseContract
    {
        public ResponseStatus Status { get; set; }

        public List<string> MessageIds { get; }

        public string ErrorDescription { get; set; }

        public MessageResponseContract()
        {
            MessageIds = new List<string>();
        }
    }
}
