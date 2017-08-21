using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    internal static class ObjectTreeTraverser
    {
        //todo: caching?
        public static TreeNode Traverse(Type type)
        {
            if (IsSimpleType(type))
            {
                return TreeNode.Empty;
            }
            
            var dictionaryInterface = FindDictionaryInterface(type);
            if (dictionaryInterface != null)
            {
                var arguments = dictionaryInterface.GetGenericArguments();
                var dictionaryValueType = arguments[1];
                return TreeNode.Dictionary(Traverse(dictionaryValueType));
            }
            
            var enumerableInterface = FindEnumerableInterface(type);
            if (enumerableInterface != null)
            {
                var arguments = enumerableInterface.GetGenericArguments();
                var enumerableElementType = arguments[0];
                return TreeNode.Collection(Traverse(enumerableElementType));
            }

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new ObjectProperty(p, Traverse(p.PropertyType)))
                .ToArray();

            return TreeNode.Object(properties);
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
    }
}