using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class ReflectionProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesByType = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<PropertyInfo, string> ResponseNameByProperty = new ConcurrentDictionary<PropertyInfo, string>();
        private static readonly ConcurrentDictionary<Type, Attribute[]> CustomAttributesByType = new ConcurrentDictionary<Type, Attribute[]>();
        private static readonly ConcurrentDictionary<Type, Type[]> InterfacesByType = new ConcurrentDictionary<Type, Type[]>();
        private static readonly ConcurrentDictionary<Type, Type> DictionaryValueTypeByType = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> EnumerableElementTypeByType = new ConcurrentDictionary<Type, Type>();
        
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

        public static bool TryGetDictionaryValueType(Type type, out Type valueType)
        {
            if (DictionaryValueTypeByType.TryGetValue(type, out valueType))
            {
                return true;
            }
            
            var dictionaryInterface = GetInterfaces(type)
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (dictionaryInterface == null)
            {
                valueType = null;
                return false;
            }

            var arguments = dictionaryInterface.GetGenericArguments();
            var dictionaryValueType = arguments[1];
            valueType = dictionaryValueType;
            DictionaryValueTypeByType.AddOrUpdate(type, t => dictionaryValueType, (t1, t2) => dictionaryValueType);
            return true;
        }
        
        public static bool TryGetEnumerableElementType(Type type, out Type valueType)
        {
            if (EnumerableElementTypeByType.TryGetValue(type, out valueType))
            {
                return true;
            }
                
            var enumerableInterface = GetInterfaces(type)
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerableInterface == null)
            {
                valueType = null;
                return false;
            }

            var arguments = enumerableInterface.GetGenericArguments();
            var enumerableElementType = arguments[0];
            valueType = enumerableElementType;
            EnumerableElementTypeByType.AddOrUpdate(type, t => enumerableElementType, (t1, t2) => enumerableElementType);
            return true;
        }

        private static Type[] GetInterfaces(Type type)
        {
            return InterfacesByType.GetOrAdd(type, t => t.GetInterfaces());
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