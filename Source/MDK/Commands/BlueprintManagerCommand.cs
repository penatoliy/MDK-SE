using System;
using Malware.MDKModules;
using Malware.MDKUI.BlueprintManager;
using MDK.Resources;
using MDK.VisualStudio;

namespace MDK.Commands
{
    sealed class BlueprintManagerCommand : ProjectDependentCommand
    {
        public BlueprintManagerCommand(ExtendedPackage package) : base(package)
        { }

        public override Guid GroupId { get; } = CommandGroups.MDKGroup;

        public override int Id { get; } = CommandIds.BlueprintManager;

        protected override void OnExecute()
        {
            var package = (MDKPackage)Package;
            if (!TryGetValidProject(out var mdkOptions))
            {
                package.Dialogs.ShowMessage(Text.BlueprintManagerCommand_OnExecute_NoMDKProjects, Text.BlueprintManagerCommand_OnExecute_NoMDKProjectsDescription, MessageType.Error);
                return;
            }
            var model = new BlueprintManagerDialogModel((IMDKWriteableOptions)package.Options, MDKPackage.HelpPageUrl)
            {
                BlueprintPath = mdkOptions.OutputPath
            };
            BlueprintManagerDialog.ShowDialog(model);
        }
    }
}
