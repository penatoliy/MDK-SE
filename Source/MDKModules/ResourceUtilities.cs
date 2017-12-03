using System;
using System.Reflection;
using System.Resources;

namespace Malware.MDKModules
{
    class ResourceUtilities
    {
        public static ResourceManager Get(Type resourceType)
        {
            if (typeof(ResourceManager).IsAssignableFrom(resourceType))
                return (ResourceManager)Activator.CreateInstance(resourceType);
            var property = resourceType.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ResourceManager), Type.EmptyTypes, null);
            if (property != null)
                return (ResourceManager)property.GetValue(null);
            throw new ArgumentException("Not a type with a static ResourceManager property in it", nameof(resourceType));
        }
    }
}