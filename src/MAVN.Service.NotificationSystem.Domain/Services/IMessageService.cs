using System.Threading.Tasks;
using MAVN.Service.NotificationSystem.Domain.Models;

namespace MAVN.Service.NotificationSystem.Domain.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// Process email message
        /// </summary>
        /// <param name="emailMessage">Email message info</param>
        /// <param name="callType">Type of the call (rest, rabbit, ...)</param>
        /// <returns></returns>
        Task<MessageResponseContract> ProcessEmailAsync(EmailMessage emailMessage, CallType callType);

        /// <summary>
        /// Process sms
        /// </summary>
        /// <param name="sms">SMS details</param>
        /// <param name="callType">Type of the call (rest, rabbit, ...)</param>
        /// <returns></returns>
        Task<MessageResponseContract> ProcessSmsAsync(Sms sms, CallType callType);

        /// <summary>
        /// Process push notification
        /// </summary>
        /// <param name="pushNotification">Push notification details</param>
        /// <param name="callType">Type of the call (rest, rabbit, ...)</param>
        /// <returns></returns>
        Task<MessageResponseContract> ProcessPushNotificationAsync(PushNotification pushNotification, CallType callType);
    }
}
