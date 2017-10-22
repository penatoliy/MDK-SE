using Malware.MDKModules.Composer;
using Malware.MDKModules.Loader;
using Malware.MDKModules.Postprocessor;
using Malware.MDKModules.Preprocessor;
using Malware.MDKModules.Publisher;

namespace Malware.MDKModules
{
    /// <summary>
    /// Helper extensions for <see cref="IModule"/> types
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Determine the <see cref="ModuleType"/> of a given <see cref="IModule"/> instance
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static ModuleType GetModuleType(this IModule module)
        {
            switch (module)
            {
                case ILoader _:
                    return ModuleType.Loader;
                case IPreprocessor _:
                    return ModuleType.Preprocessor;
                case IComposer _:
                    return ModuleType.Composer;
                case IPostprocessor _:
                    return ModuleType.Postprocessor;
                case IPublisher _:
                    return ModuleType.Publisher;
                default:
                    return ModuleType.Unknown;
            }
        }
    }
}