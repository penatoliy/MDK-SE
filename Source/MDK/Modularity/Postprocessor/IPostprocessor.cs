using System.Threading.Tasks;

namespace MDK.Modularity.Postprocessor
{
    public interface IPostprocessor: IModule
    {
        Task<string> PostprocessAsync(string script, ProjectInfo projectInfo);
    }
}
