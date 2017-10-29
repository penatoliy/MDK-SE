using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Malware.MDKModules;
using Malware.MDKModules.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Malware.MDKDefaultModules.Loader.Default
{
    [Guid("B1712252-BE57-4AAA-8622-51C7B8D18A72")]
    public class DefaultLoader : Module, ILoader
    {
        public override ModuleIdentity Identity => ModuleIdentity.For(this, "Default", "1.0.0", "Morten Aune Lyrstad");

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

        async Task<Project[]> LoadScriptProjects(string solutionFileName, string selectedProjectFileName)
        {
            try
            {
                var workspace = MSBuildWorkspace.Create();
                var solution = await workspace.OpenSolutionAsync(solutionFileName);
                var solutionDir = Path.GetDirectoryName(solutionFileName) ?? Environment.CurrentDirectory;
                var result = solution.Projects
                    .Where(project =>
                    {
                        // If no specific project name is provided, we return all projects. Otherwise we
                        // only return the requested project.

                        if (selectedProjectFileName == null)
                            return true;
                        var path = Path.Combine(solutionDir, project.FilePath.TrimStart('\\'));
                        return string.Equals(path, selectedProjectFileName, StringComparison.CurrentCultureIgnoreCase);
                    })
                    .ToArray();
                return result;
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Resources.DefaultLoader_LoadScriptProjects_Error, solutionFileName), e);
            }
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
