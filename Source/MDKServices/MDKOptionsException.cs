using System;
using System.Runtime.Serialization;

namespace Malware.MDKServices
{
    /// <summary>
    /// Exception which happens during operations in <see cref="MDKOptions"/>
    /// </summary>
    [Serializable]
    public class MDKOptionsException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="MDKOptionsException"/>
        /// </summary>
        public MDKOptionsException()
        { }

        /// <summary>
        /// Creates an instance of <see cref="MDKOptionsException"/>
        /// </summary>
        /// <param name="message"></param>
        public MDKOptionsException(string message) : base(message)
        { }

        /// <summary>
        /// Creates an instance of <see cref="MDKOptionsException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MDKOptionsException(string message, Exception inner) : base(message, inner)
        { }

        /// <summary>
        /// Creates an instance of <see cref="MDKOptionsException"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MDKOptionsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }
    }
}