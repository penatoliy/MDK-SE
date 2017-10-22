using Malware.MDKModules.Composer;
using Malware.MDKModules.Loader;
using Malware.MDKModules.Postprocessor;
using Malware.MDKModules.Preprocessor;
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
        /// A <see cref="ILoader"/> module
        /// </summary>
        Loader,

        /// <summary>
        /// A <see cref="IPreprocessor"/> module
        /// </summary>
        Preprocessor,

        /// <summary>
        /// A <see cref="IComposer"/> module
        /// </summary>
        Composer,

        /// <summary>
        /// A <see cref="IPostprocessor"/> module
        /// </summary>
        Postprocessor,

        /// <summary>
        /// A <see cref="IPublisher"/> module
        /// </summary>
        Publisher
    }
}