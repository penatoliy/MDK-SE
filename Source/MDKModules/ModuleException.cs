using System;
using System.Runtime.Serialization;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents an exception happening when executing a module.
    /// </summary>
    [Serializable]
    public class ModuleException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ModuleException"/>
        /// </summary>
        public ModuleException()
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ModuleException"/>
        /// </summary>
        /// <param name="message"></param>
        public ModuleException(string message) : base(message)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ModuleException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public ModuleException(string message, Exception inner) : base(message, inner)
        { }

        /// <summary>
        /// Creates an instance of <see cref="ModuleException"/> from a serialized state
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ModuleException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }
    }
}