using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Malware.MDKModules
{
    /// <summary>
    /// Provides information about a particular build
    /// </summary>
    public class Build
    {
        /// <summary>
        /// Creates an instance of <see cref="Build"/>
        /// </summary>
        /// <param name="project"></param>
        /// <param name="options"></param>
        public Build(Project project, MDKProjectOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            Documents = new DocumentCollection(this);
            FileName = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(project.Solution.FilePath)) ?? Environment.CurrentDirectory, project.FilePath.TrimStart('\\'));
        }

        /// <summary>
        /// The filename to the Visual Studio project to be built
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// The MDK project options
        /// </summary>
        public MDKProjectOptions Options { get; }

        /// <summary>
        /// The code analysis project
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// A list of documents eligible for processing
        /// </summary>
        public DocumentCollection Documents { get; }

        /// <summary>
        /// A list of documents eligible for processing by a given build
        /// </summary>
        public class DocumentCollection : KeyedCollection<DocumentId, Document>
        {
            readonly Build _build;

            /// <summary>
            /// Creates an instance of <see cref="DocumentCollection"/>
            /// </summary>
            /// <param name="build">The build the documents belong to</param>
            public DocumentCollection(Build build)
            {
                _build = build ?? throw new ArgumentNullException(nameof(build));
            }

            /// <inheritdoc />
            protected override DocumentId GetKeyForItem(Document item) => item.Id;

            /// <inheritdoc />
            protected override void InsertItem(int index, Document item)
            {
                if (item.Project != _build.Project)
                    throw new ArgumentException("Project mismatch", nameof(item));
                base.InsertItem(index, item);
            }

            /// <inheritdoc />
            protected override void SetItem(int index, Document item)
            {
                if (item.Project != _build.Project)
                    throw new ArgumentException("Project mismatch", nameof(item));
                base.SetItem(index, item);
            }
        }
    }
}
