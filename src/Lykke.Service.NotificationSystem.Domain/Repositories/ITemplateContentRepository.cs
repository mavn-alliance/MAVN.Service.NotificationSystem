using System.Threading.Tasks;
using Lykke.Service.NotificationSystem.Domain.Models;

namespace Lykke.Service.NotificationSystem.Domain.Repositories
{
    public interface ITemplateContentRepository
    {
        Task SaveContentAsync(string templateName, Localization localization, string templateBody);
        Task<string> GetContentAsync(string templateName, Localization local);
        Task<bool> DeleteContentAsync(string templateName, Localization local);
        Task DeleteContentWithAllLocalsAsync(string templateName, Localization local);
    }
}
