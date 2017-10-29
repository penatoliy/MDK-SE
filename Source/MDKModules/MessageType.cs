namespace Malware.MDKModules
{
    /// <summary>
    /// Describes the severity of a message
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Just an informational message
        /// </summary>
        Info,

        /// <summary>
        /// A request for confirmation
        /// </summary>
        Confirm,

        /// <summary>
        /// A request for confirmation with the ability to cancel
        /// </summary>
        ConfirmOrCancel,

        /// <summary>
        /// A warning, should be noted but not too serious
        /// </summary>
        Warning,

        /// <summary>
        /// An error, action is required or some operation was aborted
        /// </summary>
        Error
    }
}