using System;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents the configured global options for MDK
    /// </summary>
    public interface IMDKOptions
    {
        /// <summary>
        /// Determines whether the extension is a prerelease version
        /// </summary>
        bool IsPrerelease { get; }

        /// <summary>
        /// Gets the current package version
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Determines whether <see cref="GameBinPath"/> should be used rather than the automatically retrieved one.
        /// </summary>
        bool UseManualGameBinPath { get; set; }

        /// <summary>
        /// If <see cref="UseManualGameBinPath"/> is <c>true</c>, this value is used instead of the automatically retrieved path.
        /// Use <see cref="GetActualGameBinPath"/> to get the actual game binary path to use.
        /// </summary>
        string GameBinPath { get; set; }

        /// <summary>
        /// Determines whether <see cref="OutputPath"/> should be used rather than the automatically retrieved path.
        /// </summary>
        bool UseManualOutputPath { get; set; }

        /// <summary>
        /// If <see cref="UseManualOutputPath"/> is <c>true</c>, this value is used rather than the automatically retreived path.
        /// Use <see cref="GetActualOutputPath"/> to get the actual output path to use.
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        bool Minify { get; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        bool PromoteMDK { get; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        bool NotifyUpdates { get; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        bool NotifyPrereleaseUpdates { get; }

        /// <summary>
        /// Returns the actual game bin path which should be used, taking all settings into account.
        /// </summary>
        /// <returns></returns>
        string GetActualGameBinPath();

        /// <summary>
        /// Returns the actual output path which should be used, taking all settings into account.
        /// </summary>
        /// <returns></returns>
        string GetActualOutputPath();
    }
}