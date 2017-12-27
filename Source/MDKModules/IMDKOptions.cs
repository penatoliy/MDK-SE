using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        /// Determines whether <see cref="ManualGameBinPath"/> should be used rather than the automatically retrieved one.
        /// </summary>
        bool UseManualGameBinPath { get; }

        /// <summary>
        /// If <see cref="UseManualGameBinPath"/> is <c>true</c>, this value is used instead of the automatically retrieved path.
        /// Use <see cref="GetActualGameBinPath"/> to get the actual game binary path to use.
        /// </summary>
        string ManualGameBinPath { get; }

        /// <summary>
        /// Determines whether <see cref="ManualOutputPath"/> should be used rather than the automatically retrieved path.
        /// </summary>
        bool UseManualOutputPath { get; }

        /// <summary>
        /// If <see cref="UseManualOutputPath"/> is <c>true</c>, this value is used rather than the automatically retreived path.
        /// Use <see cref="GetActualOutputPath"/> to get the actual output path to use.
        /// </summary>
        string ManualOutputPath { get; }

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

        /// <summary>
        /// The reference to the default composer. A value of <c>null</c> means the built-in composer.
        /// </summary>
        MDKModuleReference DefaultComposer { get; }

        /// <summary>
        /// The reference to the default publisher. A value of <c>null</c> means the built-in publisher.
        /// </summary>
        MDKModuleReference DefaultPublisher { get; }

        /// <summary>
        /// The location of plugins to use with MDK
        /// </summary>
        IReadOnlyList<Uri> PluginLocations { get; }

        /// <summary>
        /// Determines whether the blueprint manager window will be shown after a deployment.
        /// </summary>
        bool ShowBlueprintManagerOnDeploy { get; }
    }
}