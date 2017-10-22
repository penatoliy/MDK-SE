using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Malware.MDKModules.Loader
{
    public interface ILoader: IModule
    {
        Task<ImmutableArray<Build>> LoadAsync(string solutionFileName, string selectedProjectFileName = null);
    }
}