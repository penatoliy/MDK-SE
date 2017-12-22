using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Malware.MDKModules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Malware.MDKDefaultModules.Loader.Default
{
    public class DefaultLoader : Module
    {
        public async Task<ImmutableArray<Build>> LoadAsync(string solutionFileName, string selectedProjectFileName = null)
        {
            solutionFileName = Path.GetFullPath(solutionFileName ?? throw new ArgumentNullException(nameof(solutionFileName)));
            selectedProjectFileName = selectedProjectFileName != null ? Path.GetFullPath(selectedProjectFileName) : null;

            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFileName);

            var solutionDir = Path.GetDirectoryName(solutionFileName) ?? Environment.CurrentDirectory;
            var builds = ImmutableArray.CreateBuilder<Build>();
            foreach (var project in solution.Projects)
            {
                if (!IsSelectedProject(project, solutionDir, selectedProjectFileName))
                    continue;

                var scriptInfo = LoadProjectOptions(project);
                var build = new Build(project, scriptInfo);
                foreach (var document in project.Documents)
                {
                    if (IsIgnoredDocument(document.FilePath, scriptInfo))
                        continue;
                    build.Documents.Add(document);
                }

                builds.Add(build);
            }

            return builds.ToImmutable();
        }

        bool IsSelectedProject(Project project, string solutionDir, string selectedProjectFileName)
        {
            if (selectedProjectFileName == null)
            {
                // No specific project is selected, so _all_ projects are
                return true;
            }

            var path = Path.Combine(solutionDir, project.FilePath.TrimStart('\\'));
            if (string.Equals(path, selectedProjectFileName, StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }

        MDKProjectOptions LoadProjectOptions(Project project)
        {
            try
            {
                return MDKProjectOptions.Load(project.FilePath, project.Name);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Resources.DefaultLoader_LoadProjectOptions_Error, project.FilePath), e);
            }
        }

        protected virtual bool IsIgnoredDocument(string filePath, MDKProjectOptions options)
        {
            var fileName = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fileName))
                return true;

            if (fileName.Contains(".NETFramework,Version="))
                return true;

            if (fileName.EndsWith(".debug", StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (fileName.IndexOf(".debug.", StringComparison.CurrentCultureIgnoreCase) >= 0)
                return true;

            return options.IsIgnoredFilePath(filePath);
        }
    }
}
