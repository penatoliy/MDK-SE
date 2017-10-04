using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Malware.MDKUtilities
{
    /// <summary>
    /// Utility service to retrieve information about Space Engineers (copyright Keen Software House, no affiliation)
    /// </summary>
    class SpaceEngineers
    {
        /// <summary>
        /// The <see cref="Steam"/> service
        /// </summary>
        public Steam Steam { get; } = new Steam();

        /// <summary>
        /// Determines whether SpaceEngineers exists on this computer.
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            var executable = GetInstallPath("Bin64\\SpaceEngineers.exe");
            return File.Exists(executable);
        }

        /// <summary>
        /// The Steam App ID of Space Engineers
        /// </summary>
        public const long SteamAppId = 244850;

        /// <summary>
        /// Attempts to get the install path of Space Engineers.
        /// </summary>
        /// <param name="subfolders">The desired subfolder path, if any</param>
        /// <returns></returns>
        public string GetInstallPath(params string[] subfolders)
        {
            if (!Steam.Exists())
                return null;
            var installFolder = Steam.GetInstallFolder("SpaceEngineers", "Bin64\\SpaceEngineers.exe");
            if (string.IsNullOrEmpty(installFolder))
                return null;
            if (subfolders == null || subfolders.Length == 0)
                return Path.GetFullPath(installFolder);

            subfolders = new[] {installFolder}.Concat(subfolders).ToArray();
            return Path.GetFullPath(Path.Combine(subfolders));
        }

        /// <summary>
        /// Attempts to get the default data path for Space Engineers.
        /// </summary>
        /// <param name="subfolders">The desired subfolder path, if any</param>
        /// <returns></returns>
        public string GetDataPath(params string[] subfolders)
        {
            var dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineers");
            if (subfolders == null || subfolders.Length <= 0)
                return Path.GetFullPath(dataFolder);

            subfolders = new[] {dataFolder}.Concat(subfolders).ToArray();
            return Path.GetFullPath(Path.Combine(subfolders));
        }

        public async Task RunAndWait(params string[] arguments)
        {
            if (!Steam.Exists())
                throw new InvalidOperationException("Cannot find Steam");

            var appId = SteamAppId;

            var args = new List<string>
            {
                $"-applaunch {appId}"
            };
            args.AddRange(arguments);

            var process = new Process
            {
                StartInfo =
                {
                    FileName = Steam.ExePath,
                    Arguments = string.Join(" ", args)
                }
            };
            process.Start();
            if (await ForProcess("SpaceEngineers", TimeSpan.FromSeconds(60)))
            {
                await ForProcessToEnd("SpaceEngineers", TimeSpan.MaxValue);
            }
        }

        async Task<bool> ForProcess(string processName, TimeSpan timeout)
        {
            return await Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                while (true)
                {
                    if (stopwatch.Elapsed >= timeout)
                        return false;
                    foreach (var process in Process.GetProcesses())
                        Debug.WriteLine(process.ProcessName);
                    if (Process.GetProcessesByName(processName).Length > 0)
                        return true;
                    await Task.Delay(1000);
                }
            }).ConfigureAwait(false);
        }

        async Task<bool> ForProcessToEnd(string processName, TimeSpan timeout)
        {
            return await Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                while (true)
                {
                    if (stopwatch.Elapsed >= timeout)
                        return false;
                    if (Process.GetProcessesByName(processName).Length == 0)
                        return true;
                    await Task.Delay(1000);
                }
            }).ConfigureAwait(false);
        }
    }
}
