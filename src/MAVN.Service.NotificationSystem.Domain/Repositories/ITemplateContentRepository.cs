using System.Threading.Tasks;
using MAVN.Service.NotificationSystem.Domain.Models;

namespace MAVN.Service.NotificationSystem.Domain.Repositories
{
    public interface ITemplateContentRepository
    {
        Task SaveContentAsync(string templateName, Localization localization, string templateBody);
        Task<string> GetContentAsync(string templateName, Localization local);
        Task<bool> DeleteContentAsync(string templateName, Localization local);
        Task DeleteContentWithAllLocalsAsync(string templateName, Localization local);
    }
}
