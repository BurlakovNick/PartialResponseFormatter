using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class ReflectionProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesByType = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<PropertyInfo, string> ResponseNameByProperty = new ConcurrentDictionary<PropertyInfo, string>();
        private static readonly ConcurrentDictionary<Type, Attribute[]> CustomAttributesByType = new ConcurrentDictionary<Type, Attribute[]>();

        public static Attribute[] GetCustomAttributes(Type type)
        {
            return CustomAttributesByType.GetOrAdd(type, t => t.GetCustomAttributes().ToArray());
        }

        public static MapFromContractAttribute FindMapFromContractAttribute(Type type)
        {
            return GetCustomAttributes(type)
                .FirstOrDefault(x => x.GetType() == typeof(MapFromContractAttribute)) as MapFromContractAttribute;
        }
        
        public static PropertyInfo[] GetProperties(Type type)
        {
            return PropertiesByType.GetOrAdd(type,
                t => t
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(property => !property.GetCustomAttributes().Any(ShouldIgnoreAttribute))
                    .ToArray()
            );
        }

        public static string GetPropertyResponseName(PropertyInfo propertyInfo)
        {
            return ResponseNameByProperty.GetOrAdd(propertyInfo, CalculatePropertyResponseName(propertyInfo));
        }

        private static bool ShouldIgnoreAttribute(Attribute attr)
        {
            return attr.GetType().Name == "JsonIgnoreAttribute" || attr is PartialResponseIgnoreAttribute;
        }

        private static string CalculatePropertyResponseName(PropertyInfo propertyInfo)
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
                        return customName;
                    }
                }
            }

            var partialResponsePropertyAttribute =
                customAttributes.FirstOrDefault(a => a.GetType() == typeof(PartialResponsePropertyAttribute));
            if (partialResponsePropertyAttribute != null)
            {
                var propertyAttribute = (PartialResponsePropertyAttribute) partialResponsePropertyAttribute;
                var customName = propertyAttribute.PropertyName;
                if (!string.IsNullOrEmpty(customName))
                {
                    return customName;
                }
            }

            return propertyInfo.Name;
        }
    }
}