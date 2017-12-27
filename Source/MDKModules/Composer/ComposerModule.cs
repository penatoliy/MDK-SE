using System.Threading.Tasks;

namespace Malware.MDKModules.Composer
{
    /// <summary>
    /// The job of this module is to take the content of a project
    /// and combine it into a complete Space Engineers compatible
    /// script.
    /// </summary>
    public abstract class ComposerModule : Module, IComposerModule
    {
        /// <summary>
        /// Create a Space Engineers compatible script from the given build
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public abstract Task<string> ComposeAsync(Build build);
    }
}
