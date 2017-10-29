using System.Threading.Tasks;
using Malware.MDKModules.Composer;

namespace Malware.MDKModules.Preprocessor
{
    /// <summary>
    /// The job of this module is to perform processing on a build before it is composed (see <see cref="IComposer"/>).
    /// </summary>
    public interface IPreprocessor : IModule
    {
        /// <summary>
        /// Perform processing on the given build
        /// </summary>
        /// <param name="build">The build to process</param>
        /// <returns></returns>
        Task<Build> PreprocessAsync(Build build);
    }
}