using System;
using System.Reflection;

namespace Malware.MDKModules
{
    /// <summary>
    /// A factory designed to create extension modules and peripherals
    /// </summary>
    public abstract class ModuleFactory
    {
        /// <summary>
        /// Create a factory for the given type. The type must inherit from an <see cref="IModule"/> derivate, not be abstract and have a public parameterless constructor.
        /// </summary>
        /// <param name="moduleType">The module type to create a factory for.</param>
        /// <returns></returns>
        public static ModuleFactory Create(Type moduleType)
        {
            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));
            var factoryType = typeof(ModuleFactory<>).MakeGenericType(moduleType);
            return (ModuleFactory)Activator.CreateInstance(factoryType);
        }

        /// <summary>
        /// Gets the actual type of the module this factory creates.
        /// </summary>
        public abstract Type ModuleType { get; }

        /// <summary>
        /// Creates a new module of the designated type.
        /// </summary>
        /// <returns></returns>
        public IModule Create()
        {
            CreateCore(out var module);
            return module;
        }

        /// <summary>
        /// Does the actual job of creating and initializing the new module.
        /// </summary>
        /// <param name="module"></param>
        protected abstract void CreateCore(out IModule module);
    }

    /// <inheritdoc />
    /// <typeparam name="T"></typeparam>
    public sealed class ModuleFactory<T> : ModuleFactory where T : class, IModule, new()
    {
        /// <summary>
        /// Creates a <see cref="ModuleFactory{T}"/>
        /// </summary>
        public ModuleFactory()
        {
            var moduleType = typeof(T);
            var attribute = moduleType.GetCustomAttribute<ModuleAttribute>();
            if (attribute == null)
                throw new InvalidOperationException($"{moduleType.FullName} is not a valid module: Does not have a {typeof(ModuleAttribute).FullName}");
        }

        /// <inheritdoc />
        public override Type ModuleType => typeof(T);

        /// <summary>
        /// Creates a new module of the designated type.
        /// </summary>
        /// <returns></returns>
        public new T Create() => (T)base.Create();

        /// <inheritdoc />
        protected override void CreateCore(out IModule module)
        {
            module = new T();
        }
    }
}