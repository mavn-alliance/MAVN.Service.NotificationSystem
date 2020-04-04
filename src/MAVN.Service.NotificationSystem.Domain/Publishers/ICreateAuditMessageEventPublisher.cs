using System.Threading.Tasks;
using Autofac;
using Common;
using MAVN.Service.NotificationSystem.Contract.MessageContracts;

namespace MAVN.Service.NotificationSystem.Domain.Publishers
{
    public interface ICreateAuditMessageEventPublisher : IStartable, IStopable
    {
        Task PublishAsync(CreateAuditMessageEvent message);
    }
}
