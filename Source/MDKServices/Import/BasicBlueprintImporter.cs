using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;

namespace Malware.MDKServices.Import
{
    /// <summary>
    /// A base class for simple blueprint importers that loads from a Space Engineers script file
    /// </summary>
    public abstract class BasicBlueprintImporter : IBlueprintImporter
    {
        /// <summary>
        /// Import a script blueprint from the given source path
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public ImmutableArray<ScriptFile> Import(string sourcePath, [NotNull] IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("message", nameof(sourcePath));
            }
            if (!serviceProvider.TryGetService<IScriptService>(out var scriptService))
                throw new InvalidOperationException("No script service found");

            ValidateSourcePath(sourcePath, serviceProvider);
            var script = LoadScript(sourcePath, serviceProvider);
            if (script == null)
                throw new InvalidOperationException($"Could not load a script from {sourcePath}");
            script = scriptService.WrapScript(script);
            var options = new CSharpParseOptions(LanguageVersion.CSharp6);
            var syntaxTree = CSharpSyntaxTree.ParseText(script, null, sourcePath);
            var scanner = new ScriptSeparator();

            return ImmutableArray<ScriptFile>.Empty;
        }

        class ScriptSeparator : CSharpSyntaxWalker
        {
            
        }

        /// <summary>
        /// Loads a Space Engineers script file from the given location.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        protected abstract string LoadScript(string sourcePath, IServiceProvider serviceProvider);

        /// <summary>
        /// Verifies that the given source path is valid for this importer.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="serviceProvider"></param>
        protected abstract void ValidateSourcePath(string sourcePath, IServiceProvider serviceProvider);
    }
}
