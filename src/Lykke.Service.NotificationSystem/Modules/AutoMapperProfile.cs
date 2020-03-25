using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.NotificationSystem.Client.Models.Message;
using Lykke.Service.NotificationSystem.Domain.Models;
using Lykke.Service.NotificationSystem.SubscriberContract;

namespace Lykke.Service.NotificationSystem.Modules
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SendEmailRequest, EmailMessage>();
            CreateMap<EmailMessageEvent, EmailMessage>();
            CreateMap<SendSmsRequest, Sms>();
            CreateMap<SmsEvent, Sms>();
            CreateMap<PushNotificationEvent, PushNotification>();
            CreateMap<SendPushNotificationRequest, PushNotification>();
            CreateMap<MessageResponseContract, MessageResponseModel>();
        }
    }
}
