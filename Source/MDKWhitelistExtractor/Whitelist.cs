using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sandbox.ModAPI;
using SpaceEngineers.Game;
using VRage.Scripting;

namespace Malware.MDKWhitelistExtractor
{
    class Whitelist : IExtractor
    {
        public void Invoke(CommandLine commandLine, SpaceEngineersGame game, SynchronizationContext synchronizationContext)
        {
            string[] targets;
            if (commandLine.IndexOf("-all") >= 0)
            {
                targets = new[] {"whitelist.txt"};
            }
            else
            {
                var targetsArgumentIndex = commandLine.IndexOf("-whitelistcaches");
                if (targetsArgumentIndex == -1 || targetsArgumentIndex == commandLine.Count - 1)
                    return;
                var targetsArgument = commandLine[targetsArgumentIndex + 1];
                targets = targetsArgument.Split(';');
            }

            var types = new List<string>();
            foreach (var item in MyScriptCompiler.Static.Whitelist.GetWhitelist())
            {
                if (!item.Value.HasFlag(MyWhitelistTarget.Ingame))
                    continue;
                types.Add(item.Key);
            }
            foreach (var target in targets)
                File.WriteAllText(target, string.Join(Environment.NewLine, types));
        }
    }
}