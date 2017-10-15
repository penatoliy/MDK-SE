using System.Threading.Tasks;

namespace MDK.Modularity.Composer
{
    public interface IComposer: IModule
    {
        Task<string> ComposeAsync(ProjectInfo project);
    }
}
