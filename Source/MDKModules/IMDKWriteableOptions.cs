using System;
using System.Collections.Generic;

namespace Malware.MDKModules
{
    /// <summary>
    /// The <see cref="IMDKOptions"/> in a writeable format
    /// </summary>
    public interface IMDKWriteableOptions : IMDKOptions
    {
        /// <summary>
        /// Determines whether <see cref="GameBinPath"/> should be used rather than the automatically retrieved one.
        /// </summary>
        new bool UseManualGameBinPath { get; set; }

        /// <summary>
        /// If <see cref="UseManualGameBinPath"/> is <c>true</c>, this value is used instead of the automatically retrieved path.
        /// Use <see cref="IMDKOptions.GetActualGameBinPath"/> to get the actual game binary path to use.
        /// </summary>
        new string GameBinPath { get; set; }

        /// <summary>
        /// Determines whether <see cref="OutputPath"/> should be used rather than the automatically retrieved path.
        /// </summary>
        new bool UseManualOutputPath { get; set; }

        /// <summary>
        /// If <see cref="UseManualOutputPath"/> is <c>true</c>, this value is used rather than the automatically retreived path.
        /// Use <see cref="IMDKOptions.GetActualOutputPath"/> to get the actual output path to use.
        /// </summary>
        new string OutputPath { get; set; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        new bool PromoteMDK { get; set; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        new bool NotifyUpdates { get; set; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        new bool NotifyPrereleaseUpdates { get; set; }

        /// <summary>
        /// The location of plugins to use with MDK
        /// </summary>
        new IList<Uri> PluginLocations { get; }
    }
}
