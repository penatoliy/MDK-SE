using System;
using Malware.MDKUI.BlueprintManager;
using MDK.VisualStudio;

namespace MDK.Commands
{
    sealed class GlobalBlueprintManagerCommand : Command
    {
        public GlobalBlueprintManagerCommand(ExtendedPackage package) : base(package)
        { }

        public override Guid GroupId { get; } = CommandGroups.MDKGroup;

        public override int Id { get; } = CommandIds.GlobalBlueprintManager;

        protected override void OnBeforeQueryStatus()
        { }

        protected override void OnExecute()
        {
            var package = (MDKPackage)Package;
            var model = new BlueprintManagerDialogModel
            {
                BlueprintPath = package.Options.GetActualOutputPath()
            };
            BlueprintManagerDialog.ShowDialog(model);
        }
    }
}
