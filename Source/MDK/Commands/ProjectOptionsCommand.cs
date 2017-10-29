using System;
using Malware.MDKModules;
using MDK.Resources;
using MDK.Views.Options;
using MDK.VisualStudio;

namespace MDK.Commands
{
    sealed class ProjectOptionsCommand : ProjectDependentCommand
    {
        public ProjectOptionsCommand(ExtendedPackage package) : base(package)
        { }

        public override Guid GroupId { get; } = CommandGroups.MDKGroup;

        public override int Id { get; } = CommandIds.ProjectOptions;

        protected override void OnExecute()
        {
            var package = (MDKPackage)Package;
            if (!TryGetValidProject(out var mdkOptions))
            {
                package.ShowMessage(Text.ProjectOptionsCommand_OnExecute_NoMDKProjects, Text.ProjectOptionsCommand_OnExecute_NoMDKProjectsDescription, MessageType.Error);
                return;
            }
            var scriptOptions = new ScriptOptionsDialogModel((MDKPackage)Package, mdkOptions);
            ScriptOptionsDialog.ShowDialog(scriptOptions);
        }
    }
}
