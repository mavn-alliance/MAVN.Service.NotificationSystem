using Autofac;
using Lykke.Service.NotificationSystem.AzureRepositories;
using Lykke.Service.NotificationSystem.Domain.Publishers;
using Lykke.Service.NotificationSystem.Domain.Repositories;
using Lykke.Service.NotificationSystem.Domain.Subscribers;
using Lykke.Service.NotificationSystem.DomainServices;
using Lykke.Service.NotificationSystem.Rabbit.Publishers;
using Lykke.Service.NotificationSystem.Rabbit.Subscribers;
using Lykke.Service.NotificationSystem.Settings;
using Lykke.Service.NotificationSystemAdapter.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.NotificationSystem.Modules
{
    public class ServiceModule : Module
    {
        private const string SmsExchangeName = "lykke.notificationsystem.command.sms";
        private const string PublishNotificationExchangeName = "lykke.notificationsystem.command.pushnotification";
        private const string EmailMessageExchangeName = "lykke.notificationsystem.command.emailmessage";
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            builder.RegisterModule(new MsSqlRepositories.AutofacModule(
                _appSettings.CurrentValue.NotificationSystemService.Db.MsSqlDataConnString));

            builder.RegisterModule(new AutofacModule(
                _appSettings.CurrentValue.NotificationSystemService.CommonSender.IdentityNamespace,
                _appSettings.CurrentValue.NotificationSystemService.EmailSender.EmailKey,
                _appSettings.CurrentValue.NotificationSystemService.SmsSender.PhoneNumberKey,
                _appSettings.CurrentValue.NotificationSystemService.CommonSender.LocalizationKey));

            // Repositories
            builder.Register(container =>
                    new TemplateContentRepository(_appSettings.CurrentValue.NotificationSystemService.Db
                        .TemplateConnString))
                .As<ITemplateContentRepository>()
                .SingleInstance();

            // Clients
            builder.RegisterNotificationSystemAdapterClient(
                _appSettings.CurrentValue.NotificationSystemAdapterServiceClient, null);

            //Rabbit
            builder.RegisterType<BrokerMessageEventPublisher>()
                .As<IBrokerMessageEventPublisher>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", _appSettings.CurrentValue.Rabbit.ConnectionString);

            builder.RegisterType<EmailMessageSubscriber>()
                .As<IEmailMessageSubscriber>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", _appSettings.CurrentValue.Rabbit.ConnectionString)
                .WithParameter("exchangeName", EmailMessageExchangeName)
                .WithParameter("queueName", _appSettings.CurrentValue.Rabbit.EmailMessageSubscriberQueueName);

            builder.RegisterType<SmsSubscriber>()
                .As<ISmsSubscriber>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", _appSettings.CurrentValue.Rabbit.ConnectionString)
                .WithParameter("exchangeName", SmsExchangeName)
                .WithParameter("queueName", _appSettings.CurrentValue.Rabbit.SmsSubscriberQueueName);

            builder.RegisterType<PushNotificationSubscriber>()
                .As<IPushNotificationsSubscriber>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", _appSettings.CurrentValue.Rabbit.ConnectionString)
                .WithParameter("exchangeName", PublishNotificationExchangeName)
                .WithParameter("queueName", _appSettings.CurrentValue.Rabbit.PushNotificationSubscriberQueueName);

            builder.RegisterType<CreateAuditMessageEventPublisher>()
                .As<ICreateAuditMessageEventPublisher>()
                .As<IStartable>()
                .SingleInstance()
                .WithParameter("connectionString", _appSettings.CurrentValue.Rabbit.ConnectionString);
        }
    }
}
