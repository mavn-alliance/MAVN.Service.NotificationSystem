using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.NotificationSystem.Domain.Models;

namespace MAVN.Service.NotificationSystem.Domain.Services
{
    public interface ITemplateService
    {
        /// <summary>
        /// Create or update teamplate
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="templateBody">Body of template</param>
        /// <param name="localization">Language of body</param>
        Task CreateOrUpdateTemplateAsync(string templateName, string templateBody, Localization localization);

        /// <summary>
        /// Get info about template
        /// </summary>
        /// <param name="templateName">Name of template</param>
        Task<NotificationTemplateInfo> GetTemplateInfoAsync(string templateName);

        /// <summary>
        /// Get info about all templates
        /// </summary>        
        Task<IReadOnlyList<NotificationTemplateInfo>> GetTemplateInfoListAsync();

        /// <summary>
        /// Get info about template
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="local">Localization of template</param>
        Task<NotificationTemplateContent> FindTemplateContentAsync(string templateName, Localization local);

        /// <summary>
        /// Delete localization template
        /// </summary>
        Task DeleteIfExistAsync(string templateName, Localization local);

        /// <summary>
        /// Delete template with all localizations
        /// </summary>
        Task DeleteIfExistAsync(string templateName);
    }
}
