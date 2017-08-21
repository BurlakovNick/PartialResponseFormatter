using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class ReflectionProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesByType = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static object GetPropertyValue(object obj, string fieldName)
        {
            if (obj == null || string.IsNullOrEmpty(fieldName))
            {
                return obj;
            }
            
            var property = PropertiesByType
                .GetOrAdd(obj.GetType(), t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .FirstOrDefault(p => p.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));

            return property?.GetValue(obj);
        }
    }
}