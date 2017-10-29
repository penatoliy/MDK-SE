namespace Malware.MDKModules
{
    /// <summary>
    /// Describes the response of a message
    /// </summary>
    public enum MessageResponse
    {
        /// <summary>
        /// Message is accepted
        /// </summary>
        Accept,

        /// <summary>
        /// Message is rejected
        /// </summary>
        Reject,

        /// <summary>
        /// Cancel request
        /// </summary>
        Cancel
    }
}