using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using MDK.Build;
using MDK.Resources;

namespace MDK.Modularity.Publisher.Default
{
    public class DefaultPublisher: Module, IPublisher
    {
        public async Task PublishAsync(string script, ProjectInfo projectInfo)
        {
            var project = projectInfo.Project;
            var output = projectInfo.ScriptInfo.OutputPath;
            try
            {
                var outputInfo = new DirectoryInfo(MDK.ExpandMacros(projectInfo, Path.Combine(output, project.Name)));
                if (!outputInfo.Exists)
                    outputInfo.Create();
                File.WriteAllText(Path.Combine(outputInfo.FullName, "script.cs"), script.Replace("\r\n", "\n"), Encoding.UTF8);

                using (var stream = File.OpenWrite(Path.Combine(outputInfo.FullName, "script.cs")))
                {
                    var writer = new StreamWriter(stream, Encoding.UTF8);
                    await writer.WriteAsync(script.Replace("\r\n", "\n"));
                }

                var thumbFile = new FileInfo(Path.Combine(Path.GetDirectoryName(project.FilePath) ?? ".", "thumb.png"));
                if (thumbFile.Exists)
                {
                    await Task.Run(() => thumbFile.CopyTo(Path.Combine(outputInfo.FullName, "thumb.png"), true));
                }
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException(string.Format(Text.BuildModule_WriteScript_UnauthorizedAccess, project.FilePath), e);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.BuildModule_WriteScript_Error, project.FilePath), e);
            }
        }
    }
}
