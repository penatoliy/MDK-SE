using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDK.Modularity.Preprocessor
{
    public interface IPreprocessor: IModule
    {
        Task<ProjectInfo> PreprocessAsync(ProjectInfo projectInfo);
    }
}
