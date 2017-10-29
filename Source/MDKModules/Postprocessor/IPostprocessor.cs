using System.Threading.Tasks;
using Malware.MDKModules.Composer;

namespace Malware.MDKModules.Postprocessor
{
    /// <summary>
    /// The job of this module is to perform processing on a composed script (see <see cref="IComposer"/>).
    /// </summary>
    public interface IPostprocessor: IModule
    {
        /// <summary>
        /// Perform processing on the given script
        /// </summary>
        /// <param name="script">The composed script</param>
        /// <param name="build">The build this script was constructed from</param>
        /// <returns></returns>
        Task<string> PostprocessAsync(string script, Build build);
    }
}
