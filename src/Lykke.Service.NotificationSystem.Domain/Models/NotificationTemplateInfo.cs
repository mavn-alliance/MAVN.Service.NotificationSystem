using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.NotificationSystem.Domain.Models
{
    /// <summary>
    /// Template info
    /// </summary>
    public class NotificationTemplateInfo
    {
        public NotificationTemplateInfo(string name, List<Localization> availableLocalizations)
        {
            Name = name;
            AvailableLocalizations = availableLocalizations.ToList();
        }

        /// <summary>
        /// Template name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of available localizations
        /// </summary>
        public List<Localization> AvailableLocalizations { get; }

        public bool HasLocalization(Localization localization)
        {
            return AvailableLocalizations.Any(e => e.Equals(localization));
        }
    }
}
