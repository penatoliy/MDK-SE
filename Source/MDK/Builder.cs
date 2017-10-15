using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MDK.Build;
using MDK.Modularity;
using MDK.Modularity.Composer;
using MDK.Modularity.Composer.Default;
using MDK.Modularity.Loader;
using MDK.Modularity.Loader.Default;
using MDK.Modularity.Postprocessor;
using MDK.Modularity.Preprocessor;
using MDK.Modularity.Publisher;
using MDK.Modularity.Publisher.Default;
using MDK.Resources;

namespace MDK
{
    public class Builder
    {
        /// <summary>
        /// The associated MDK services
        /// </summary>
        public IMDK MDK { get; }

        /// <summary>
        /// Creates a new instance of <see cref="Builder"/>
        /// </summary>
        /// <param name="mdk"></param>
        public Builder(IMDK mdk)
        {
            MDK = mdk;
        }

        /// <summary>
        /// The loader which will be used if no <see cref="CustomLoader"/> is specified.
        /// </summary>
        public ILoader DefaultLoader { get; } = new DefaultLoader();
        /// <summary>
        /// An optional custom loader.
        /// </summary>
        public ILoader CustomLoader { get; set; }

        /// <summary>
        /// A list of preprocessors which works on the project before it's composed into a script.
        /// </summary>
        public List<IPreprocessor> Preprocessors { get; } = new List<IPreprocessor>();

        /// <summary>
        /// The composer which will be used if no <see cref="CustomComposer"/> is specified.
        /// </summary>
        public IComposer DefaultComposer { get; } = new DefaultComposer();

        /// <summary>
        /// An optional custom composer.
        /// </summary>
        public IComposer CustomComposer { get; set; }

        /// <summary>
        /// A list of postprocessors which works on the script created by the composer.
        /// </summary>
        public List<IPostprocessor> Postprocessors { get; } = new List<IPostprocessor>();

        /// <summary>
        /// The publisher which will be used if ho <see cref="CustomPublisher"/> is specified.
        /// </summary>
        public IPublisher DefaultPublisher { get; } = new DefaultPublisher();

        /// <summary>
        /// An optional custom publisher.
        /// </summary>
        public IPublisher CustomPublisher { get; set; }

        /// <summary>
        /// Builds the provided solution; optionally a single project within that solution.
        /// </summary>
        /// <param name="solutionFileName">The file name of the solution to build</param>
        /// <param name="selectedProjectFileName">An optional project file name to build within <see cref="solutionFileName"/></param>
        /// <param name="progressReport">An optional object where progress will be reported</param>
        /// <returns></returns>
        public async Task Build(string solutionFileName, string selectedProjectFileName = null, IProgress<float> progressReport = null)
        {
            solutionFileName = Path.GetFullPath(solutionFileName);
            var loader = CustomLoader ?? DefaultLoader;
            var preprocessors = Preprocessors.ToArray();
            var composer = CustomComposer ?? DefaultComposer;
            var postprocessors = Postprocessors.ToArray();
            var publisher = CustomPublisher ?? DefaultPublisher;
            var synchronizationContext = SynchronizationContext.Current;

            loader.Begin(MDK);
            foreach (var preprocessor in preprocessors)
                preprocessor.Begin(MDK);
            composer.Begin(MDK);
            foreach (var postprocessor in postprocessors)
                postprocessor.Begin(MDK);
            publisher.Begin(MDK);

            try
            {
                ImmutableArray<ProjectInfo> projects;
                try
                {
                    projects = await loader.LoadAsync(solutionFileName, selectedProjectFileName);
                }
                catch (Exception e)
                {
                    throw new BuildException(string.Format(Text.BuildModule_LoadScriptProjects_Error, solutionFileName), e);
                }

                if (projects.Length == 0)
                    return;

                var progress = new Progress(projects.Length * (3 + preprocessors.Length + postprocessors.Length), progressReport, synchronizationContext);
                await Task.WhenAll(projects.Select(project => Build(progress, project, preprocessors, composer, postprocessors, publisher)));

                loader.End();
                foreach (var preprocessor in preprocessors)
                    preprocessor.End();
                composer.End();
                foreach (var postprocessor in postprocessors)
                    postprocessor.Begin(null);
                publisher.End();
            }
            catch (Exception e)
            {
                loader.End(e);
                foreach (var preprocessor in preprocessors)
                    preprocessor.End(e);
                composer.End(e);
                foreach (var postprocessor in postprocessors)
                    postprocessor.End(e);
                publisher.End(e);
                throw;
            }
        }

        async Task Build(Progress progress, ProjectInfo project, IEnumerable<IPreprocessor> preprocessors, IComposer composer, IEnumerable<IPostprocessor> postprocessors, IPublisher publisher)
        {
            progress.Advance();
            foreach (var processor in preprocessors)
            {
                project = await processor.PreprocessAsync(project);
                progress.Advance();
            }

            var script = await composer.ComposeAsync(project);
            progress.Advance();

            foreach (var processor in postprocessors)
            {
                script = await processor.PostprocessAsync(script, project);
                progress.Advance();
            }

            await publisher.PublishAsync(script, project);
        }
    }
}
