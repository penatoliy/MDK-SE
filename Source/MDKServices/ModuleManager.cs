using System;
using System.Collections.Immutable;
using Malware.MDKDefaultModules.Composer.Default;
using Malware.MDKDefaultModules.Loader.Default;
using Malware.MDKDefaultModules.Publisher.Default;
using Malware.MDKModules;

namespace Malware.MDKServices
{
    /// <summary>
    ///     A manager class that keeps track of available deployment modules
    /// </summary>
    public class ModuleManager
    {
        Lazy<ModuleIdentity> _lazyDefaultLoader = new Lazy<ModuleIdentity>(ModuleIdentity.For<DefaultLoader>);
        Lazy<ModuleIdentity> _lazyDefaultComposer = new Lazy<ModuleIdentity>(ModuleIdentity.For<DefaultComposer>);
        Lazy<ModuleIdentity> _lazyDefaultPublisher = new Lazy<ModuleIdentity>(ModuleIdentity.For<DefaultPublisher>);

        /// <summary>
        ///     The identity of the default loader module
        /// </summary>
        public ModuleIdentity DefaultLoader => _lazyDefaultLoader.Value;

        /// <summary>
        ///     The identity of the default composer module
        /// </summary>
        public ModuleIdentity DefaultComposer => _lazyDefaultComposer.Value;

        /// <summary>
        ///     The identity of the default publisher module
        /// </summary>
        public ModuleIdentity DefaultPublisher => _lazyDefaultPublisher.Value;

        /// <summary>
        ///     Loads any additional deployment modules.
        /// </summary>
        /// <param name="includeDefault">Whether the default modules should be included in the load</param>
        /// <returns></returns>
        public ImmutableArray<ModuleIdentity> LoadDeploymentPlugins(bool includeDefault = true)
        {
            var builder = ImmutableArray.CreateBuilder<ModuleIdentity>();
            if (includeDefault)
            {
                builder.Add(DefaultLoader);
                builder.Add(DefaultComposer);
                builder.Add(DefaultPublisher);
            }
            // TODO load custom modules
            return builder.ToImmutableArray();
        }
    }
}
