using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDK.Build;
using MDK.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MDK.Modularity.Composer.Default
{
    public class DefaultComposer: Module, IComposer
    {
        public DocumentAnalyzer Analyzer { get; } = new DocumentAnalyzer();

        /// <summary>
        /// Gets the comparer used to determine the order of parts (files) when building.
        /// </summary>
        public IComparer<ScriptPart> PartComparer { get; } = new WeightedPartSorter();

        public async Task<string> ComposeAsync(ProjectInfo project)
        {
            var content = await LoadProjectContent(project);
            var document = CreateProgramDocument(project.Project, content);
            return await GenerateScript(project.Project, document);
        }

        async Task<ProjectContent> LoadProjectContent(ProjectInfo project)
        {
            var usingDirectives = ImmutableArray.CreateBuilder<UsingDirectiveSyntax>();
            var parts = ImmutableArray.CreateBuilder<ScriptPart>();
            var documents = project.Documents.ToList();

            var readmeDocument = project.Documents
                .Where(document => Path.GetDirectoryName(document.FilePath)?.Equals(Path.GetDirectoryName(project.FileName), StringComparison.CurrentCultureIgnoreCase) ?? false)
                .FirstOrDefault(document => Path.GetFileNameWithoutExtension(document.FilePath).Equals("readme", StringComparison.CurrentCultureIgnoreCase));

            string readme = null;
            if (readmeDocument != null)
            {
                documents.Remove(readmeDocument);
                readme = (await readmeDocument.GetTextAsync()).ToString().Replace("\r\n", "\n");
                if (!readme.EndsWith("\n"))
                    readme += "\n";
            }

            foreach (var document in documents)
            {
                var result = await Analyzer.Analyze(document).ConfigureAwait(false);
                if (result == null)
                    continue;
                usingDirectives.AddRange(result.UsingDirectives);
                parts.AddRange(result.Parts);
            }

            var comparer = new UsingDirectiveComparer();
            return new ProjectContent(usingDirectives.Distinct(comparer).ToImmutableArray(), parts.ToImmutable(), readme);
        }

        Document CreateProgramDocument(Project project, ProjectContent content)
        {
            try
            {
                var usings = string.Join(Environment.NewLine, content.UsingDirectives.Select(d => d.ToString()));
                var solution = project.Solution;

                var buffer = new StringBuilder();
                buffer.Append("public class Program: MyGridProgram {");
                buffer.Append(string.Join("", content.Parts.OfType<ProgramScriptPart>().OrderBy(part => part, PartComparer).Select(p => p.GenerateContent())));
                buffer.Append("}");
                var programContent = buffer.ToString();

                buffer.Clear();
                buffer.Append(string.Join("", content.Parts.OfType<ExtensionScriptPart>().OrderBy(part => part, PartComparer).Select(p => p.GenerateContent())));
                var extensionContent = buffer.ToString();

                var finalContent = $"{usings}\n{programContent}\n{extensionContent}";

                var compilationProject = solution.AddProject("__ScriptCompilationProject", "__ScriptCompilationProject.dll", LanguageNames.CSharp)
                    .WithCompilationOptions(project.CompilationOptions)
                    .WithMetadataReferences(project.MetadataReferences);

                return compilationProject.AddDocument("Program.cs", finalContent);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.BuildModule_CreateProgramDocument_Error, project.FilePath), e);
            }
        }

        async Task<string> GenerateScript(Project project, Document document)
        {
            try
            {
                var generator = new ScriptGenerator();
                var script = await generator.Generate(document).ConfigureAwait(false);
                return script;
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.BuildModule_GenerateScript_ErrorGeneratingScript, project.FilePath), e);
            }
        }
    }
}
