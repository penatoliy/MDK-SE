using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Malware.MDKUtilities;
using Malware.MDKWhitelistExtractor;

namespace DocGen
{
    public class Program
    {
        public static int Main()
        {
            var commandLine = new CommandLine(Environment.CommandLine);
            try
            {
                var program = new Program();
                return Task.Run(async () => await program.Run(commandLine).ConfigureAwait(false)).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                if (commandLine.IndexOf("-verbose") >= 0)
                    Console.WriteLine(e);
                else
                    Console.WriteLine(e.Message);
                return -1;
            }
        }

        async Task UpdateCaches(string path)
        {
            var steam = new Steam();
            if (!steam.Exists())
                throw new InvalidOperationException("Cannot find Steam");
            var spaceEngineers = new SpaceEngineers();
            if (!spaceEngineers.Exists())
                throw new InvalidOperationException("Cannot find Space Engineers");

            var pluginPath = Path.GetFullPath("MDKWhitelistExtractor.dll");
            var whitelistTarget = path;
            var terminalTarget = path;
            var namespaceTarget = path;
            var directoryInfo = new DirectoryInfo(whitelistTarget);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            whitelistTarget = Path.Combine(whitelistTarget, "whitelist.cache");
            terminalTarget = Path.Combine(terminalTarget, "terminal.cache");
            namespaceTarget = Path.Combine(namespaceTarget, "namespaces.cache");

            await spaceEngineers.RunAndWait(
                $"-plugin \"{pluginPath}\"",
                "-nosplash",
                $"-whitelistcaches \"{whitelistTarget}\"",
                $"-terminalcaches \"{terminalTarget}\"",
                $"-namespacecaches \"{namespaceTarget}\""
            );
        }

        async Task<int> Run(CommandLine commandLine)
        {
            var path = Environment.CurrentDirectory;
            var cacheIndex = commandLine.IndexOf("-cache");
            if (cacheIndex >= 0)
                path = Path.GetFullPath(commandLine[cacheIndex + 1]);

            var update = commandLine.IndexOf("-update") >= 0;

            if (update)
                await UpdateCaches(path);

            string output = null;
            var outputIndex = commandLine.IndexOf("-output");
            if (outputIndex >= 0)
                output = Path.GetFullPath(commandLine[outputIndex + 1]);
            if (output != null)
                GenerateDocs(path, output);

            return 0;
        }

        void GenerateDocs(string path, string output)
        {
            //var whitelistTarget = path;
            var terminalTarget = path;
            //whitelistTarget = Path.Combine(whitelistTarget, "whitelist.cache");
            terminalTarget = Path.Combine(terminalTarget, "terminal.cache");

            var terminals = Terminals.Load(terminalTarget);
            terminals.Save(Path.Combine(output, "List-Of-Terminal-Properties-And-Actions.md"));

            //var directoryInfo = new DirectoryInfo(output);
            //if (!directoryInfo.Exists)
            //    directoryInfo.Create();

        }
    }
}