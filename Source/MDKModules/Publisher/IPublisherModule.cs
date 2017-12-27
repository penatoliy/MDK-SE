using System.Threading.Tasks;
using Malware.MDKModules.Composer;

namespace Malware.MDKModules.Publisher
{
    /// <summary>
    /// The job of this module is to take a composed script (see <see cref="IComposerModule"/>) and put it somewhere
    /// the game can get to it.
    /// </summary>
    public interface IPublisherModule : IModule
    {
        /// <summary>
        /// Publish the given script
        /// </summary>
        /// <param name="script">The composed script</param>
        /// <param name="build">The build this script was composed from</param>
        /// <returns></returns>
        Task PublishAsync(string script, Build build);
    }
}