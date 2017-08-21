using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class ResponseSpecificationFactory
    {
        public static ResponseSpecificationBuilder Create()
        {
            return new ResponseSpecificationBuilder();
        }
        
        public static ResponseSpecification Create<T>()
        {
            var type = typeof(T);
            return new ResponseSpecification
            {
                Fields = SelectFields(type)
            };
        }

        private static Field[] SelectFields(Type type)
        {
            if (IsSimpleType(type))
            {
                return new Field[0];
            }
            
            var dictionaryInterface = FindDictionaryInterface(type);
            if (dictionaryInterface != null)
            {
                var arguments = dictionaryInterface.GetGenericArguments();
                return SelectFields(arguments[1]);
            }
            
            var enumerableInterface = FindEnumerableInterface(type);
            if (enumerableInterface != null)
            {
                var arguments = enumerableInterface.GetGenericArguments();
                return SelectFields(arguments[0]);
            }

            //todo: validate duplicates
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(BuildField)
                .ToArray();
        }

        private static Field BuildField(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var propertyName = FieldNameProvider.GetPropertyName(property);
            
            if (IsSimpleType(propertyType))
            {
                return CreateField(propertyName);
            }
            
            var dictionaryInterface = FindDictionaryInterface(propertyType);
            if (dictionaryInterface != null)
            {
                var arguments = dictionaryInterface.GetGenericArguments();
                return CreateField(propertyName, SelectFields(arguments[1]));
            }
            
            var enumerableInterface = FindEnumerableInterface(propertyType);
            if (enumerableInterface != null)
            {
                var arguments = enumerableInterface.GetGenericArguments();
                return CreateField(propertyName, SelectFields(arguments[0]));
            }

            return CreateField(propertyName, SelectFields(propertyType));
        }

        //todo: optimize all reflection
        private static Type FindDictionaryInterface(Type type)
        {
            var interfaces = type.GetInterfaces();

            return interfaces
                .FirstOrDefault(x => x.IsGenericType &&
                                     x.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                );
        }
        
        //todo: optimize all reflection
        private static Type FindEnumerableInterface(Type type)
        {
            var interfaces = type.GetInterfaces();

            return interfaces
                .FirstOrDefault(x => x.IsGenericType &&
                                     x.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                );
        }
        
        private static bool IsSimpleType(Type type)
        {
            return type == typeof(string) ||
                   type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(decimal) ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ||
                   type == typeof(DateTime);
        }

        private static Field CreateField(string name, Field[] subFields = null)
        {
            return new Field
            {
                Name = name,
                Fields = subFields ?? new Field[0]
            };
        }
    }
}