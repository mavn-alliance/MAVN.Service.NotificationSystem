using Autofac;
using AutoMapper;
using Lykke.Service.NotificationSystem.Domain.Services;

namespace Lykke.Service.NotificationSystem.DomainServices
{
    public class AutofacModule : Module
    {
        private readonly string _identityNamespace;
        private readonly string _emailKey;
        private readonly string _phoneNumber;
        private readonly string _localizationKey;

        public AutofacModule(string identityNamespace, string emailKey, string phoneNumber, string localizationKey)
        {
            _identityNamespace = identityNamespace;
            _emailKey = emailKey;
            _phoneNumber = phoneNumber;
            _localizationKey = localizationKey;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Mapper>()
                .As<IMapper>()
                .SingleInstance();

            builder.RegisterType<MessageService>()
                .As<IMessageService>()
                .WithParameter("identityNamespace", _identityNamespace)
                .WithParameter("emailKey", _emailKey)
                .WithParameter("phoneNumberKey", _phoneNumber)
                .WithParameter("localizationKey", _localizationKey)
                .SingleInstance();

            builder.RegisterType<TemplateService>()
                .As<ITemplateService>()
                .SingleInstance();
        }
    }
}
