using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MDK.Modularity.Loader
{
    public interface ILoader: IModule
    {
        Task<ImmutableArray<ProjectInfo>> LoadAsync(string solutionFileName, string selectedProjectFileName = null);
    }
}