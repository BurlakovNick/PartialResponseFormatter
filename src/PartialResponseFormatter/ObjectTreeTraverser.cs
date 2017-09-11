using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    internal static class ObjectTreeTraverser
    {
        private static readonly ConcurrentDictionary<Type, TreeNode> TreeNodesByType = new ConcurrentDictionary<Type, TreeNode>();
        
        public static TreeNode Traverse<T>()
        {
            return Traverse(typeof(T));
        }
        
        public static TreeNode Traverse(Type type)
        {
            var traverseStack = new Stack<Type>();
            return TraverseWithCaching(type, traverseStack);
        }

        private static TreeNode TraverseWithCaching(Type type, Stack<Type> traverseStack)
        {
            if (TreeNodesByType.TryGetValue(type, out var treeNode))
            {
                return treeNode;
            }
            
            if (traverseStack.Contains(type))
            {
                var traversePath = string.Join(",", traverseStack.Select(x => x.Name));
                throw new InvalidOperationException($"Cyclic dependency found while tree traversing. " +
                                                    $"Looping type: {type.Name}. " +
                                                    $"Traversing stack: {traversePath}");
            }
            traverseStack.Push(type);
            var result = TreeNodesByType.GetOrAdd(type, t => TraverseWithoutCaching(t, traverseStack));
            traverseStack.Pop();
            return result;
        }

        private static TreeNode TraverseWithoutCaching(Type type, Stack<Type> traverseStack)
        {
            if (IsSimpleType(type))
            {
                return TreeNode.Empty;
            }

            if (ReflectionProvider.TryGetDictionaryValueType(type, out var valueType))
            {
                return TreeNode.Dictionary(TraverseWithCaching(valueType, traverseStack));
            }

            if (ReflectionProvider.TryGetEnumerableElementType(type, out var elementType))
            {
                return TreeNode.Collection(TraverseWithCaching(elementType, traverseStack));
            }

            var properties = ReflectionProvider.GetProperties(type)
                .Select(p => new ObjectProperty(p, TraverseWithCaching(p.PropertyType, traverseStack)))
                .ToArray();

            return TreeNode.Object(properties);
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