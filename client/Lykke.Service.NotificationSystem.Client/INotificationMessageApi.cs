using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.NotificationSystem.Client.Models.Message;
using Refit;

namespace Lykke.Service.NotificationSystem.Client
{
    /// <summary>
    /// NotificationSystem client API interface for work with messages
    /// </summary>
    [PublicAPI]
    public interface INotificationMessageApi
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="model">Info about the email</param>
        /// <returns></returns>
        [Post("/api/message/email")]
        Task<MessageResponseModel> SendEmailAsync(SendEmailRequest model);

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="model">Info about the sms</param>
        /// <returns></returns>
        [Post("/api/message/sms")]
        Task<MessageResponseModel> SendSmsAsync(SendSmsRequest model);

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="model">Info about the sms</param>
        /// <returns></returns>
        [Post("/api/message/pushNotification")]
        Task<MessageResponseModel> SendPushNotificationAsync(SendPushNotificationRequest model);
    }
}
