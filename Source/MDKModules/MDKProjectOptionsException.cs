using System;
using System.Runtime.Serialization;

namespace Malware.MDKModules
{
    /// <summary>
    /// Exception which happens during operations in <see cref="MDKProjectOptions"/>
    /// </summary>
    [Serializable]
    public class MDKProjectOptionsException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="MDKProjectOptionsException"/>
        /// </summary>
        public MDKProjectOptionsException()
        { }

        /// <summary>
        /// Creates an instance of <see cref="MDKProjectOptionsException"/>
        /// </summary>
        /// <param name="message"></param>
        public MDKProjectOptionsException(string message) : base(message)
        { }

        /// <summary>
        /// Creates an instance of <see cref="MDKProjectOptionsException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MDKProjectOptionsException(string message, Exception inner) : base(message, inner)
        { }

        /// <summary>
        /// Creates an instance of <see cref="MDKProjectOptionsException"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MDKProjectOptionsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }
    }
}