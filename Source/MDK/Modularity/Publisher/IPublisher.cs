using System.Threading.Tasks;

namespace MDK.Modularity.Publisher
{
    public interface IPublisher: IModule
    {
        Task PublishAsync(string script, ProjectInfo projectInfo);
    }
}
