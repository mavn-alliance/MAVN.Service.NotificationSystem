using System.Collections.Generic;
using Lykke.Service.NotificationSystem.Domain.Enums;

namespace Lykke.Service.NotificationSystem.Domain.Models
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
