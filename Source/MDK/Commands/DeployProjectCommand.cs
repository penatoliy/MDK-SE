using System;
using Malware.MDKModules;
using MDK.Resources;
using MDK.VisualStudio;

namespace MDK.Commands
{
    sealed class DeployProjectCommand : ProjectDependentCommand
    {
        public DeployProjectCommand(ExtendedPackage package) : base(package)
        { }

        public override Guid GroupId { get; } = CommandGroups.MDKGroup;

        public override int Id { get; } = CommandIds.DeployProject;

        protected override async void OnExecute()
        {
            var package = (MDKPackage)Package;
            if (!TryGetValidProject(out var project, out _))
            {
                package.Dialogs.ShowMessage(Text.DeployProjectCommand_OnExecute_NoMDKProjects, Text.DeployProjectCommand_OnExecute_NoMDKProjectsDescription, MessageType.Error);
                return;
            }
            await package.Deploy(project);
        }
    }
}
