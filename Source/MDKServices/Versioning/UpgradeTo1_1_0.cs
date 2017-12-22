using System;
using Malware.MDKModules;

#pragma warning disable 618

namespace Malware.MDKServices.Versioning
{
    // ReSharper disable once InconsistentNaming
    sealed class UpgradeTo1_1_0 : Upgrader
    {
        public override void Upgrade(MDKProjectOptions projectOptions)
        {
            if (projectOptions.Minify)
            {
                projectOptions.Minify = false;
                projectOptions.ComposerModule = new MDKModuleReference(new Guid("F565AF35-15DF-4C64-B83A-2B5F8510D948"));
            }
        }
    }
}
