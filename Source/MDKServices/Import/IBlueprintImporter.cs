using System;
using System.Collections.Immutable;

namespace Malware.MDKServices.Import
{
    /// <summary>
    /// Represents a script blueprint importer.
    /// </summary>
    public interface IBlueprintImporter
    {
        /// <summary>
        /// Import a script blueprint from the given source path
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        ImmutableArray<ScriptFile> Import(string sourcePath, IServiceProvider serviceProvider);
    }
}