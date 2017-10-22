using System.Threading.Tasks;

namespace Malware.MDKModules.Preprocessor
{
    public interface IPreprocessor: IModule
    {
        Task<Build> PreprocessAsync(Build build);
    }
}
