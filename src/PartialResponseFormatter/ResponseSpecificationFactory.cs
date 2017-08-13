using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestPartialResponse
{
    public class ResponseSpecificationFactory : IResponseSpecificationFactory
    {
        //todo: doesn't work with collections in root
        public ResponseSpecification Create<T>()
        {
            var type = typeof(T);
            return new ResponseSpecification
            {
                Fields = SelectFields(type)
            };
        }

        private Field[] SelectFields(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = new List<Field>();
            foreach (var property in properties)
            {
                fields.Add(BuildField(property));
            }
            return fields.ToArray();
        }

        private Field BuildField(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            if (IsSimpleType(propertyType))
            {
                return CreateField(property.Name);
            }

            var interfaces = propertyType.GetInterfaces();
            
            var dictionaryInterface = interfaces
                .FirstOrDefault(x => x.IsGenericType &&
                                     x.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                );
            if (dictionaryInterface != null)
            {
                var arguments = dictionaryInterface.GetGenericArguments();
                return CreateField(property.Name, SelectFields(arguments[1]));
            }
            
            var enumerableInterface = interfaces
                .FirstOrDefault(x => x.IsGenericType &&
                                     x.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                );
            if (enumerableInterface != null)
            {
                var arguments = enumerableInterface.GetGenericArguments();
                return CreateField(property.Name, SelectFields(arguments[0]));
            }

            return CreateField(property.Name, SelectFields(propertyType));
        }

        private bool IsSimpleType(Type type)
        {
            return type == typeof(string) ||
                   type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(decimal) ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ||
                   type == typeof(DateTime);
        }

        private Field CreateField(string name, Field[] subFields = null)
        {
            return new Field
            {
                Name = name,
                Fields = subFields ?? new Field[0]
            };
        }
    }
}