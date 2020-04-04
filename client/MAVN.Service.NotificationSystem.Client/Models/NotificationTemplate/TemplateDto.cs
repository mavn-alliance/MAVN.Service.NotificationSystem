namespace MAVN.Service.NotificationSystem.Client.Models.NotificationTemplate
{
    /// <summary>
    /// Full info about notification template
    /// </summary>
    public class TemplateDto
    {
        /// <summary>
        /// Unique name of a template
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Body of the template.Can be big text or file.
        /// </summary>
        public string TemplateBody { get; set; }

        /// <summary>
        /// Localization code
        /// </summary>
        public LanguageDto LocalizationCode { get; set; }
    }
}
