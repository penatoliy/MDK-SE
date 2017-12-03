using System;
using System.Threading.Tasks;
using Malware.MDKModules;
using Malware.MDKModules.Postprocessor;

namespace Malware.MDKDefaultModules.Postprocessor.Minifier
{
    [Module("524C0973-EF72-46C7-A38C-D3A6D64ECA6D",
        ResourceManagerType = typeof(Resources),
        TitleResourceKey = nameof(Resources.Minifier_Title),
        DescriptionResourceKey = nameof(Resources.Minifier_Description),
        Version = "2.0.0",
        Author = "Morten \"Malware\" Aune Lyrstad")]
    public class Minifier : Module, IPostprocessor
    {
        public Task<string> PostprocessAsync(string script, Build build)
        {
            throw new NotImplementedException();
        }
    }
}
