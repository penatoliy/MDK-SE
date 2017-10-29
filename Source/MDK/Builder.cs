using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Malware.MDKDefaultModules.Composer.Default;
using Malware.MDKDefaultModules.Loader.Default;
using Malware.MDKDefaultModules.Publisher.Default;
using Malware.MDKModules;
using Malware.MDKModules.Composer;
using Malware.MDKModules.Loader;
using Malware.MDKModules.Postprocessor;
using Malware.MDKModules.Preprocessor;
using Malware.MDKModules.Publisher;
using MDK.Resources;

namespace MDK
{
    /// <summary>
    /// Uses the various configured build modules to generate a final Space Engineers ingame script from a Visual Studio project.
    /// </summary>
    public class Builder
    {
        static void InvokeModule<T>(T module, string operation, Action<T> action) where T: class, IModule
        {
            try
            {
                action(module);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.Builder_InvokeModule_Error, operation, module.GetModuleType(), module.Identity.Title), e);
            }
        }

        static async Task<TResult> InvokeModule<T, TResult>(T module, string operation, Func<T, Task<TResult>> function) where T: class, IModule
        {
            try
            {
                return await function(module);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.Builder_InvokeModule_Error, operation, module.GetModuleType(), module.Identity.Title), e);
            }
        }

        static async Task InvokeModule<T>(T module, string operation, Func<T, Task> function) where T: class, IModule
        {
            try
            {
                await function(module);
            }
            catch (Exception e)
            {
                throw new BuildException(string.Format(Text.Builder_InvokeModule_Error, operation, module.GetModuleType(), module.Identity.Title), e);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="Builder"/>
        /// </summary>
        /// <param name="mdk"></param>
        public Builder(IMDK mdk)
        {
            MDK = mdk;
        }

        /// <summary>
        /// The associated MDK services
        /// </summary>
        public IMDK MDK { get; }

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
        /// <param name="selectedProjectFileName">An optional project file name to build within the given solution</param>
        /// <param name="progressReport">An optional object where progress will be reported</param>
        /// <returns></returns>
        public async Task<ImmutableArray<Build>> Build(string solutionFileName, string selectedProjectFileName = null, IProgress<float> progressReport = null)
        {
            solutionFileName = Path.GetFullPath(solutionFileName);
            var loader = CustomLoader ?? DefaultLoader;
            var preprocessors = Preprocessors.ToArray();
            var composer = CustomComposer ?? DefaultComposer;
            var postprocessors = Postprocessors.ToArray();
            var publisher = CustomPublisher ?? DefaultPublisher;
            var synchronizationContext = SynchronizationContext.Current;

            var modules = ImmutableArray<IModule>.Empty
                .Add(loader)
                .AddRange(preprocessors)
                .Add(composer)
                .AddRange(postprocessors)
                .Add(publisher);

            MDK.OutputPane.WriteLine("Starting build...");
            foreach (var module in modules)
                InvokeModule(module, nameof(module.BeginBatch), m => m.BeginBatch(MDK));

            try
            {
                MDK.OutputPane.WriteLine($"{loader.Identity} is loading the build...");
                var builds = await InvokeModule(loader, nameof(loader.LoadAsync), m => m.LoadAsync(solutionFileName, selectedProjectFileName));
                MDK.OutputPane.WriteLine($"{loader.Identity} loaded {builds.Length} build(s).");
                if (builds.Length == 0)
                    return ImmutableArray<Build>.Empty;

                var progress = new Progress(builds.Length * (3 + preprocessors.Length + postprocessors.Length), progressReport, synchronizationContext);
                await Task.WhenAll(builds.Select(project => Build(progress, project, preprocessors, composer, postprocessors, publisher)));

                foreach (var module in modules)
                    InvokeModule(module, nameof(module.EndBatch), m => m.EndBatch());

                MDK.OutputPane.WriteLine("Build complete.");
                return builds;
            }
            catch (Exception e)
            {
                MDK.OutputPane.WriteLine($"Build failed: {e.Message}");
                foreach (var module in modules)
                    InvokeModule(module, nameof(module.EndBatch), m => m.EndBatch(e));
                throw;
            }
        }

        async Task Build(Progress progress, Build build, IEnumerable<IPreprocessor> preprocessors, IComposer composer, IEnumerable<IPostprocessor> postprocessors, IPublisher publisher)
        {
            progress.Advance();
            foreach (var processor in preprocessors)
            {
                MDK.OutputPane.WriteLine($"{processor.Identity} is working...");
                build = await InvokeModule(processor, nameof(processor.PreprocessAsync), m => m.PreprocessAsync(build));
                progress.Advance();
            }

            MDK.OutputPane.WriteLine($"{composer.Identity} is working...");
            var script = await InvokeModule(composer, nameof(composer.ComposeAsync), m => m.ComposeAsync(build));
            progress.Advance();

            foreach (var processor in postprocessors)
            {
                MDK.OutputPane.WriteLine($"{processor.Identity} is working...");
                script = await InvokeModule(processor, nameof(processor.PostprocessAsync), m => m.PostprocessAsync(script, build));
                progress.Advance();
            }

            MDK.OutputPane.WriteLine($"{publisher.Identity} is working...");
            await InvokeModule(publisher, nameof(publisher.PublishAsync), m => m.PublishAsync(script, build));
        }
    }
}
