using System;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Common.ApiLibrary.Filters;
using MAVN.Service.NotificationSystem.Modules;
using MAVN.Service.NotificationSystem.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MAVN.Service.NotificationSystem
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "NotificationSystem API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.Extend = (serviceCollection, settings) =>
                {
                    serviceCollection.AddAutoMapper(
                        typeof(DomainServices.AutoMapperProfile),
                        typeof(AutoMapperProfile)
                    );
                };

                options.ConfigureMvcOptions = b => b.Filters.Add(typeof(NoContentFilter));

                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "NotificationSystemLog";
                    logs.AzureTableConnectionStringResolver =
                        settings => settings.NotificationSystemService.Db.LogsConnString;
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IMapper mapper)
        {
            app.UseLykkeConfiguration(options => { options.SwaggerOptions = _swaggerOptions; });

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
