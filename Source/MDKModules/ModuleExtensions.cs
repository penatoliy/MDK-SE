using Malware.MDKModules.Composer;
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
                case IComposer _:
                    return ModuleType.Composer;
                case IPublisher _:
                    return ModuleType.Publisher;
                default:
                    return ModuleType.Unknown;
            }
        }
    }
}