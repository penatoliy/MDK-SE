using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Malware.MDKServices
{
    public class ServiceProvider: IServiceProvider
    {
        Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public void RegisterService([NotNull] Type serviceType, [NotNull] object service)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (serviceType.IsValueType)
                throw new ArgumentException("Value types cannot be service types", nameof(serviceType));
            if (!serviceType.IsInstanceOfType(service))
                throw new ArgumentException("The provided service is not of the service type", nameof(service));
            _services[serviceType] = service;
        }

        public void RegisterService<T>(T instance) where T: class
        {
            RegisterService(typeof(T), instance);
        }

        public object GetService(Type serviceType)
        {
            _services.TryGetValue(serviceType, out var service);
            return service;
        }
    }
}