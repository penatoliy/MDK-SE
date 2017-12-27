using Malware.MDKModules.Composer;
using Malware.MDKModules.Publisher;

namespace Malware.MDKModules
{
    /// <summary>
    /// Determines the type of a module
    /// </summary>
    public enum ModuleType
    {
        /// <summary>
        /// Indeterminate module type (usually an error state)
        /// </summary>
        Unknown,

        /// <summary>
        /// A <see cref="IComposerModule"/> module
        /// </summary>
        Composer,

        /// <summary>
        /// A <see cref="IPublisherModule"/> module
        /// </summary>
        Publisher
    }
}