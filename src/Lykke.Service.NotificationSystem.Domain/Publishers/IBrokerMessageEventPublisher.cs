using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Service.NotificationSystem.Contract.MessageContracts;

namespace Lykke.Service.NotificationSystem.Domain.Publishers
{
    public interface IBrokerMessageEventPublisher : IStartable, IStopable
    {
        Task PublishAsync(BrokerMessage message);
    }
}
