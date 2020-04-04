using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.NotificationSystem.Domain.Models;

namespace MAVN.Service.NotificationSystem.Domain.Repositories
{
    public interface ITemplateRepository
    {
        Task<NotificationTemplateInfo> GetTemplateInfoAsync(string templateName);
        Task InsertOrUpdateAsync(NotificationTemplateInfo templateInfo);
        Task<List<NotificationTemplateInfo>> GetListTemlateInfoAsync();
        Task DeleteTemplateLocalizationAsync(string temaplateName, Localization local);
        Task DeleteTemplateAsync(string templateName);
    }
}
