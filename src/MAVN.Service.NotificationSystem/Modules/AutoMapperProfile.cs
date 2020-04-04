using AutoMapper;
using JetBrains.Annotations;
using MAVN.Service.NotificationSystem.Client.Models.Message;
using MAVN.Service.NotificationSystem.Domain.Models;
using MAVN.Service.NotificationSystem.SubscriberContract;

namespace MAVN.Service.NotificationSystem.Modules
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
