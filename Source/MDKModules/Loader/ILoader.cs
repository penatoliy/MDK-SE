using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Malware.MDKModules.Loader
{
    /// <summary>
    /// The job of this module is to load the given solution (optionally a specific project in that solution).
    /// </summary>
    /// <remarks>
    /// Loader modules can only be configured at the global level (Tools | Options | MDK/SE), because it's
    /// responsible for loading the individual project options which will contain the selection of the other 
    /// individual modules.
    /// </remarks>
    public interface ILoader: IModule
    {
        /// <summary>
        /// Load the given solution
        /// </summary>
        /// <param name="solutionFileName">The file name of the solution to load</param>
        /// <param name="selectedProjectFileName">An optional specific project to load</param>
        /// <returns>One or more builds representing the selected projects</returns>
        Task<ImmutableArray<Build>> LoadAsync(string solutionFileName, string selectedProjectFileName = null);
    }
}