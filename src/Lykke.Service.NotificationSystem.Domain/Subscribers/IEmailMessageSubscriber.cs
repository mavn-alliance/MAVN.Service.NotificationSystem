using Autofac;
using Common;

namespace Lykke.Service.NotificationSystem.Domain.Subscribers
{
    public interface IEmailMessageSubscriber : IStartable, IStopable
    {
    }
}
