using System;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents the identity of a module
    /// </summary>
    public struct ModuleIdentity
    {
        /// <summary>
        /// The unique ID of this module
        /// </summary>
        public readonly Guid Id;

        /// <summary>
        /// The human-readable title of this module
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// The current module version
        /// </summary>
        public readonly string Version;

        /// <summary>
        /// The author of this module
        /// </summary>
        public readonly string Author;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleIdentity"/>
        /// </summary>
        /// <param name="id">The unique ID of this module</param>
        /// <param name="title">The human-readable title of this module</param>
        /// <param name="version">The current module version</param>
        /// <param name="author">The author of this module</param>
        public ModuleIdentity(Guid id, string title, string version, string author)
        {
            Id = id;
            Title = title;
            Version = version;
            Author = author;
        }
    }
}