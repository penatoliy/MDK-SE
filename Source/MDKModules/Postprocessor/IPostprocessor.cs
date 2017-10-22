using System.Threading.Tasks;

namespace Malware.MDKModules.Postprocessor
{
    public interface IPostprocessor: IModule
    {
        Task<string> PostprocessAsync(string script, Build build);
    }
}
