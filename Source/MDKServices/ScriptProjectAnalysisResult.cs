using System;
using System.Collections.Immutable;
using System.Xml.Linq;
using Malware.MDKModules;

namespace Malware.MDKServices
{
    /// <summary>
    /// Represents the results for a single project of an analysis made by <see cref="ScriptUpgrades.Analyze(EnvDTE.Project,ScriptUpgradeAnalysisOptions)"/>.
    /// </summary>
    public class ScriptProjectAnalysisResult
    {
        /// <summary>
        /// Represents the results of an analysis which was ignored and should be disregarded.
        /// </summary>
        public static readonly ScriptProjectAnalysisResult NonScriptProjectResult = new ScriptProjectAnalysisResult(null, null, null, null, default(WhitelistReference), ImmutableArray<BadReference>.Empty);

        /// <summary>
        /// Creates a new instance of <see cref="ScriptProjectAnalysisResult"/>
        /// </summary>
        /// <param name="expectedVersion">The expected options version</param>
        /// <param name="project"></param>
        /// <param name="options">Basic information about the analyzed project</param>
        /// <param name="projectDocument">The source XML document of the project file</param>
        /// <param name="whitelist">Whitelist verification results</param>
        /// <param name="badReferences">A list of bad file- or assembly references</param>
        public ScriptProjectAnalysisResult(Version expectedVersion, EnvDTE.Project project, MDKProjectOptions options, XDocument projectDocument, WhitelistReference whitelist, ImmutableArray<BadReference> badReferences)
        {
            ExpectedVersion = expectedVersion;
            Project = project;
            Options = options;
            ProjectDocument = projectDocument;
            BadReferences = badReferences;
            Whitelist = whitelist;
            IsScriptProject = options != null;
            IsValid = BadReferences.Length == 0 && whitelist.IsValid;
        }

        /// <summary>
        /// This is not a script project and should be ignored.
        /// </summary>
        public bool IsScriptProject { get; }

        /// <summary>
        /// The expected package version
        /// </summary>
        public Version ExpectedVersion { get; }

        /// <summary>
        /// The actual version as found in the options
        /// </summary>
        public Version ActualVersion => Options?.Version;

        /// <summary>
        /// The DTE project
        /// </summary>
        public EnvDTE.Project Project { get; }

        /// <summary>
        /// Basic information about the analyzed project.
        /// </summary>
        public MDKProjectOptions Options { get; }

        /// <summary>
        /// The source XML document of the project file.
        /// </summary>
        public XDocument ProjectDocument { get; }

        /// <summary>
        /// Returns a list of bad file- or assembly references.
        /// </summary>
        public ImmutableArray<BadReference> BadReferences { get; }

        /// <summary>
        /// Whitelist verification result
        /// </summary>
        public WhitelistReference Whitelist { get; }

        /// <summary>
        /// Determines whether the analyzed project is fully valid and do not require any updates.
        /// </summary>
        public bool IsValid { get; }
    }
}
