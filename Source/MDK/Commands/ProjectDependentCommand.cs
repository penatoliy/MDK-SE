using System.Collections;
using System.Linq;
using EnvDTE;
using Malware.MDKModules;
using Malware.MDKServices;
using MDK.Services;
using MDK.VisualStudio;
using Command = MDK.VisualStudio.Command;

namespace MDK.Commands
{
    abstract class ProjectDependentCommand : Command
    {
        protected ProjectDependentCommand(ExtendedPackage package) : base(package)
        { }

        protected override void OnBeforeQueryStatus()
        {
            var package = (MDKPackage)Package;
            OleCommand.Visible = package.IsEnabled && TryGetValidProject(out _);
        }

        protected bool TryGetValidProject(out Project project, out MDKProjectOptions projectOptions)
        {
            var dte2 = (EnvDTE80.DTE2)Package.DTE;
            project = ((IEnumerable)dte2.ToolWindows.SolutionExplorer.SelectedItems)
                .OfType<UIHierarchyItem>()
                .Select(item => item.Object)
                .OfType<Project>()
                .FirstOrDefault();
            if (project == null)
            {
                projectOptions = null;
                return false;
            }
            projectOptions = MDKProjectOptions.Load(project.FullName, project.Name);
            return projectOptions.IsValid;
        }

        protected bool TryGetValidProject(out MDKProjectOptions projectOptions)
            => TryGetValidProject(out _, out projectOptions);
    }
}