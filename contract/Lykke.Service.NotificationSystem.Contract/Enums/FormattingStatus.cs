namespace Lykke.Service.NotificationSystem.Contract.Enums
{
    /// <summary>
    /// Represents message formatting status
    /// </summary>
    public enum FormattingStatus
    {
        /// <summary>
        /// All message parameters are filled
        /// </summary>
        Success,

        /// <summary>
        /// There are some message parameters that could not be filled (system could not find value)
        /// </summary>
        ValueNotFound
    }
}
