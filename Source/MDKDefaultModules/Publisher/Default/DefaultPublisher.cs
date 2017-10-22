using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Malware.MDKModules;
using Malware.MDKModules.Publisher;

namespace Malware.MDKDefaultModules.Publisher.Default
{
    public class DefaultPublisher : Module, IPublisher
    {
        public async Task PublishAsync(string script, Build build)
        {
            var project = build.Project;
            var output = build.Options.OutputPath;
            try
            {
                var outputInfo = new DirectoryInfo(MDK.ExpandMacros(build, Path.Combine(output, project.Name)));
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
                throw new UnauthorizedAccessException(string.Format(Resources.DefaultPublisher_PublishAsync_UnauthorizedAccess, project.FilePath), e);
            }
            catch (Exception e)
            {
                throw new Malware.MDKModules.BuildException(string.Format(Resources.DefaultPublisher_PublishAsync_Error, project.FilePath), e);
            }
        }

        public override ModuleIdentity Identity => new ModuleIdentity(new Guid("2B0754B0-A9B3-48EA-869C-7B04C6A93A40"), "Default", "1.0.0", "Morten Aune Lyrstad");
    }
}
