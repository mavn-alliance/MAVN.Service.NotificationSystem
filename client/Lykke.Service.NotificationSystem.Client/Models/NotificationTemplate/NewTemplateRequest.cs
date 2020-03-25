namespace Lykke.Service.NotificationSystem.Client.Models.NotificationTemplate
{
    /// <summary>
    /// Dto for create or update notification template
    /// </summary>
    public class NewTemplateRequest
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
        /// Localization in format {lang code ISO 639-1}-{ang region ISO 3166-1, optional}
        /// Use LocalizationCode = "*" for set default localization
        /// </summary>
        public string LocalizationCode { get; set; }
    }
}
