using System;
using System.IO;
using System.Linq;
using System.Threading;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game;
using VRage.Scripting;

namespace Malware.MDKWhitelistExtractor
{
    class WrapperTemplate : IExtractor
    {
        public void Invoke(CommandLine commandLine, SpaceEngineersGame game, SynchronizationContext synchronizationContext)
        {
            string[] targets;
            if (commandLine.IndexOf("-all") >= 0)
            {
                targets = new[] {"wrapper.txt"};
            }
            else
            {
                var targetsArgumentIndex = commandLine.IndexOf("-wrappertemplate");
                if (targetsArgumentIndex == -1 || targetsArgumentIndex == commandLine.Count - 1)
                    return;
                var targetsArgument = commandLine[targetsArgumentIndex + 1];
                targets = targetsArgument.Split(';');
            }

            var script = MyScriptCompiler.Static.GetIngameScript("<#=Content#>", "Program", typeof(MyGridProgram).FullName);

            foreach (var target in targets)
                File.WriteAllText(target, script.Code);
        }
    }
}