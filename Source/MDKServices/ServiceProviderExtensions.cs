using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Malware.MDKServices
{
    /// <summary>
    /// Provides extra helper methods for <see cref="IServiceProvider"/>
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="serviceProvider">The source <see cref="IServiceProvider"/></param>
        /// <returns>A service object of the requested type or <c>null</c> if there is no service object of that type.</returns>
        public static T GetService<T>([NotNull] this IServiceProvider serviceProvider) where T: class
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            return (T)serviceProvider.GetService(typeof(T));
        }

        /// <summary>
        /// Attempts to retrieve the service object of the specified type.
        /// </summary>
        /// <param name="serviceProvider">The source <see cref="IServiceProvider"/></param>
        /// <param name="serviceType">The type of service object to get.</param>
        /// <param name="service">A service object of the requested type or <c>null</c> if there is no service object of that type.</param>
        /// <returns><c>true</c> if the service was retrieved, <c>false</c> otherwise.</returns>
        public static bool TryGetService(this IServiceProvider serviceProvider, Type serviceType, out object service)
        {
            service = serviceProvider.GetService(serviceType);
            return service != null;
        }

        /// <summary>
        /// Attempts to retrieve the service object of the specified type.
        /// </summary>
        /// <param name="serviceProvider">The source <see cref="IServiceProvider"/></param>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="service">A service object of the requested type or <c>null</c> if there is no service object of that type.</param>
        /// <returns><c>true</c> if the service was retrieved, <c>false</c> otherwise.</returns>
        public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T service) where T: class
        {
            service = serviceProvider.GetService<T>();
            return service != null;
        }
    }
}
