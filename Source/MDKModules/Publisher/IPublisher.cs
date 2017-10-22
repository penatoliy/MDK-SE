using System.Threading.Tasks;

namespace Malware.MDKModules.Publisher
{
    public interface IPublisher: IModule
    {
        Task PublishAsync(string script, Build build);
    }
}
