using JetBrains.Annotations;

namespace Lykke.Service.NotificationSystem.Client
{
    /// <summary>
    /// NotificationSystem client interface.
    /// </summary>
    [PublicAPI]
    public interface INotificationSystemClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - INotificationTemplateApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        INotificationTemplateApi Api { get; }
    }
}
