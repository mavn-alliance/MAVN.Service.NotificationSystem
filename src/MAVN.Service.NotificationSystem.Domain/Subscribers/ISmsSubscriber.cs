using Autofac;
using Common;

namespace MAVN.Service.NotificationSystem.Domain.Subscribers
{
    public interface ISmsSubscriber : IStartable, IStopable
    {
        
    }
}
