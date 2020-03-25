using AutoMapper;

namespace Lykke.Service.NotificationSystem.Tests
{
    public static class Helpers
    {
        public static IMapper CreateAutoMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(DomainServices.AutoMapperProfile)));

            return config.CreateMapper();
        }
    }
}
