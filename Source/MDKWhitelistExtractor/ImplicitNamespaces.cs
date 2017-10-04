using System;
using System.IO;
using System.Linq;
using System.Threading;
using SpaceEngineers.Game;
using VRage.Scripting;

namespace Malware.MDKWhitelistExtractor
{
    class ImplicitNamespaces : IExtractor
    {
        public void Invoke(CommandLine commandLine, SpaceEngineersGame game, SynchronizationContext synchronizationContext)
        {
            var targetsArgumentIndex = commandLine.IndexOf("-namespacecaches");
            if (targetsArgumentIndex == -1 || targetsArgumentIndex == commandLine.Count - 1)
                return;
            var targetsArgument = commandLine[targetsArgumentIndex + 1];
            var targets = targetsArgument.Split(';');

            var namespaces = MyScriptCompiler.Static.ImplicitIngameScriptNamespaces.ToArray();
            foreach (var target in targets)
                File.WriteAllText(target, string.Join(Environment.NewLine, namespaces));
        }
    }
}