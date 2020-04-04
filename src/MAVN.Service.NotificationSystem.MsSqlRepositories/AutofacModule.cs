using Autofac;
using Lykke.Common.MsSql;
using MAVN.Service.NotificationSystem.Domain.Repositories;

namespace MAVN.Service.NotificationSystem.MsSqlRepositories
{
    public class AutofacModule : Module
    {
        private readonly string _connectionString;

        public AutofacModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _connectionString,
                connString => new NotificationSystemContext(connString, false),
                dbConn => new NotificationSystemContext(dbConn));

            builder.RegisterType<TemplateRepository>()
                .As<ITemplateRepository>()
                .SingleInstance();
        }
    }
}
