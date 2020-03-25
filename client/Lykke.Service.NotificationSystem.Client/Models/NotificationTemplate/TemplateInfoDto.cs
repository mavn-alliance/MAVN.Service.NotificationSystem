using System.Collections.Generic;

namespace Lykke.Service.NotificationSystem.Client.Models.NotificationTemplate
{
    /// <summary>
    /// Notification template description
    /// </summary>
    public class TemplateInfoDto
    {
        /// <summary>
        /// Unique name of a template
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// List of available languages
        /// </summary>
        public List<LanguageDto> AvailableLocalizations { get; set; }
    }
}
