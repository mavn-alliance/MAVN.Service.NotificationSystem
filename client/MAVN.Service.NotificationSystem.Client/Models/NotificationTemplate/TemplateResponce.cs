namespace MAVN.Service.NotificationSystem.Client.Models.NotificationTemplate
{
    /// <summary>
    /// Responce for get template
    /// </summary>
    public class TemplateResponce
    {
        /// <summary>
        /// Template data.
        /// If template npt exist then null.
        /// </summary>
        public TemplateDto Template { get; set; }
    }
}
