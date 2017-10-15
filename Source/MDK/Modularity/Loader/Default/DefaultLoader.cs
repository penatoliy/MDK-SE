using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Malware.MDKServices;
using MDK.Build;
using MDK.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace MDK.Modularity.Loader.Default
{
    public class DefaultLoader : Module, ILoader
    {
        public async Task<ImmutableArray<ProjectInfo>> LoadAsync(string solutionFileName, string selectedProjectFileName = null)
        {
            solutionFileName = Path.GetFullPath(solutionFileName ?? throw new ArgumentNullException(nameof(solutionFileName)));
            selectedProjectFileName = selectedProjectFileName != null ? Path.GetFullPath(selectedProjectFileName) : null;

            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFileName);

            var solutionDir = Path.GetDirectoryName(solutionFileName) ?? Environment.CurrentDirectory;
            var projects = ImmutableArray.CreateBuilder<ProjectInfo>();
            foreach (var project in solution.Projects)
            {
                if (IsSelectedProject(project, solutionDir, selectedProjectFileName))
                    continue;

                var scriptInfo = LoadScriptInfo(project);
                var projectInfo = new ProjectInfo(project, scriptInfo);
                foreach (var document in project.Documents)
                {
                    if (IsIgnoredDocument(document.FilePath, scriptInfo))
                        continue;
                    projectInfo.Documents.Add(document);
                }
            }

            return projects.ToImmutable();
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
                throw new BuildException(string.Format(Text.BuildModule_LoadScriptProjects_Error, solutionFileName), e);
            }
        }

        ProjectScriptInfo LoadScriptInfo(Project project)
        {
            try
            {
                return ProjectScriptInfo.Load(project.FilePath, project.Name);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.BuildModule_LoadConfig_Error, project.FilePath), e);
            }
        }

        protected virtual bool IsIgnoredDocument(string filePath, ProjectScriptInfo config)
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

            return config.IsIgnoredFilePath(filePath);
        }
    }
}
