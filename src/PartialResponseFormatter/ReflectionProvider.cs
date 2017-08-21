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
        
        public static string GetPropertyResponseName(PropertyInfo propertyInfo)
        {
            var customAttributes = propertyInfo.GetCustomAttributes().ToArray();
            var jsonPropertyAttribute = customAttributes.FirstOrDefault(a => a.GetType().Name == "JsonPropertyAttribute");
            if (jsonPropertyAttribute != null)
            {
                var jsonPropertyName = jsonPropertyAttribute
                    .GetType()
                    .GetProperty("PropertyName", BindingFlags.Instance | BindingFlags.Public);
                if (jsonPropertyName != null)
                {
                    var customName = (string) jsonPropertyName.GetValue(jsonPropertyAttribute);
                    if (!string.IsNullOrEmpty(customName))
                    {
                        return customName.ToLower();
                    }
                }
            }

            var partialResponsePropertyAttribute = customAttributes.FirstOrDefault(a => a.GetType() == typeof(PartialResponsePropertyAttribute));
            if (partialResponsePropertyAttribute != null)
            {
                var propertyAttribute = (PartialResponsePropertyAttribute)partialResponsePropertyAttribute;
                var customName = propertyAttribute.PropertyName;
                if (!string.IsNullOrEmpty(customName))
                {
                    return customName.ToLower();
                }
            }

            return propertyInfo.Name.ToLower();
        }
    }
}