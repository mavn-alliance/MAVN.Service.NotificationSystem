using System.Threading.Tasks;
using Autofac;
using Common;
using MAVN.Service.NotificationSystem.Contract.MessageContracts;

namespace MAVN.Service.NotificationSystem.Domain.Publishers
{
    public interface IBrokerMessageEventPublisher : IStartable, IStopable
    {
        Task PublishAsync(BrokerMessage message);
    }
}
