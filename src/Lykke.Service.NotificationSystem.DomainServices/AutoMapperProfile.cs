using System;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.NotificationSystem.Contract.Enums;
using Lykke.Service.NotificationSystem.Contract.MessageContracts;
using Lykke.Service.NotificationSystem.Domain.Models;

namespace Lykke.Service.NotificationSystem.DomainServices
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<EmailMessage, CreateAuditMessageEvent>()
                .ForMember(dest => dest.FormattingStatus, opt => opt.MapFrom(src => FormattingStatus.Success))
                .ForMember(dest => dest.CallType, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.MessageGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.FormattingComment, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => Channel.Email))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));
             
            CreateMap<Sms, CreateAuditMessageEvent>()
                .ForMember(dest => dest.SubjectTemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.FormattingStatus, opt => opt.MapFrom(src => FormattingStatus.Success))
                .ForMember(dest => dest.CallType, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.MessageGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.FormattingComment, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => Channel.Sms))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<PushNotification, CreateAuditMessageEvent>()
                .ForMember(dest => dest.SubjectTemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.FormattingStatus, opt => opt.MapFrom(src => FormattingStatus.Success))
                .ForMember(dest => dest.CallType, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.MessageGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.FormattingComment, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => Channel.PushNotification))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
