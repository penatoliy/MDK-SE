using System;
using System.Collections.ObjectModel;
using System.IO;
using JetBrains.Annotations;
using Malware.MDKServices;
using Microsoft.CodeAnalysis;

namespace MDK.Modularity
{
    public class ProjectInfo
    {
        public ProjectInfo([NotNull] Project project, [NotNull] ProjectScriptInfo scriptInfo)
        {
            ScriptInfo = scriptInfo ?? throw new ArgumentNullException(nameof(scriptInfo));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            Documents = new DocumentCollection(this);
            FileName = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(project.Solution.FilePath)) ?? Environment.CurrentDirectory, project.FilePath.TrimStart('\\'));
        }

        public string FileName { get; }

        public ProjectScriptInfo ScriptInfo { get; }

        public Project Project { get; }

        public SolutionInfo SolutionInfo { get; }

        public DocumentCollection Documents { get; }

        public class DocumentCollection : KeyedCollection<DocumentId, Document>
        {
            readonly ProjectInfo _projectInfo;

            public DocumentCollection(ProjectInfo projectInfo)
            {
                _projectInfo = projectInfo;
            }

            /// <inheritdoc />
            protected override DocumentId GetKeyForItem(Document item) => item.Id;

            /// <inheritdoc />
            protected override void InsertItem(int index, Document item)
            {
                if (item.Project != _projectInfo.Project)
                    throw new ArgumentException("Project mismatch", nameof(item));
                base.InsertItem(index, item);
            }

            /// <inheritdoc />
            protected override void SetItem(int index, Document item)
            {
                if (item.Project != _projectInfo.Project)
                    throw new ArgumentException("Project mismatch", nameof(item));
                base.SetItem(index, item);
            }
        }
    }
}
