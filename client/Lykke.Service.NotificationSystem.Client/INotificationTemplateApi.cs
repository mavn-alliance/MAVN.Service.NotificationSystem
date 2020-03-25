using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.NotificationSystem.Client.Models.NotificationTemplate;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Lykke.Service.NotificationSystem.Client
{
    /// <summary>
    /// NotificationSystem client API interface for work with templates.
    /// </summary>
    [PublicAPI]
    public interface INotificationTemplateApi
    {
        /// <summary>
        /// Create template
        /// </summary>
        /// <param name="template">info about template</param>
        [Post("/api/templates")]
        Task CreateTemplateAsync([FromBody] NewTemplateRequest template);

        /// <summary>
        /// Update template
        /// </summary>
        /// <param name="template">info about template</param>
        [Put("/api/templates")]
        Task UpdateTemplateAsync([FromBody] NewTemplateRequest template);

        /// <summary>
        /// Get info about all templates.
        /// Info not include template body.
        /// </summary>
        /// <returns>List of template descriptions</returns>
        [Get("/api/templates")]
        Task<List<TemplateInfoDto>> GetTemplateteRegistryAsync();

        /// <summary>
        /// Get full info template by name and language code.
        /// </summary>
        /// <param name="templateName">Name of template</param>
        /// <param name="templateLanguage">Language of template</param>
        /// <returns>Template with content(body)</returns>
        [Get("/api/templates/{templateName}/{templateLanguage}")]
        Task<TemplateResponce> FindTemplateAsync([FromRoute] string templateName, [FromRoute] string templateLanguage);

        /// <summary>
        /// Delete template by name, with all language
        /// </summary>
        /// <param name="templateName">Name of template</param>
        [Delete("/api/templates/{templateName}")]
        Task DeleteTemplateAsync([FromRoute] string templateName);

        /// <summary>
        /// Delete template by name and language
        /// </summary>
        /// <param name="templateName">>Name of template</param>
        /// <param name="templateLanguage">Template language</param>
        /// <returns></returns>
        [Delete("/api/templates/{templateName}/{templateLanguage}")]
        Task DeleteTemplateAsync([FromRoute] string templateName, [FromRoute] string templateLanguage);
    }
}
