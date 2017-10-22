using System.Threading.Tasks;

namespace Malware.MDKModules.Composer
{
    public interface IComposer: IModule
    {
        Task<string> ComposeAsync(Build project);
    }
}
