namespace MAVN.Service.NotificationSystem.Contract.Enums
{
    /// <summary>
    /// Represent how message was sent (type of the call)
    /// </summary>
    public enum CallType
    {
        /// <summary>
        /// Message came through REST API
        /// </summary>
        Rest,

        /// <summary>
        /// Message came through RabbitMQ
        /// </summary>
        RabbitMq
    }
}
