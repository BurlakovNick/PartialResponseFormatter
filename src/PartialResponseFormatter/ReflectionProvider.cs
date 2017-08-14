using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class ReflectionProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesByType = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
            {
                return obj;
            }
            
            var property = PropertiesByType
                .GetOrAdd(obj.GetType(), t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            return property?.GetValue(obj);
        }
    }
}