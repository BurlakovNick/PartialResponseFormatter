using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class ResponseSpecificationFactory
    {
        private static readonly Field[] Empty = new Field[0];

        public static ResponseSpecificationBuilder Create()
        {
            return new ResponseSpecificationBuilder();
        }

        public static ResponseSpecification Create<T>()
        {
            var type = typeof(T);
            var treeNode = ObjectTreeTraverser.Traverse(type);

            return new ResponseSpecification
            {
                Fields = SelectFields(treeNode)
            };
        }

        private static Field[] SelectFields(TreeNode treeNode)
        {            
            switch (treeNode)
            {
                case null:
                case EmptyTreeNode _:
                    return Empty;
                case ObjectTreeNode obj:
                    return SelectObject(obj);
                case CollectionTreeNode collection:
                    return SelectFields(collection.Items);
                case DictionaryTreeNode dictionary:
                    return SelectFields(dictionary.Items);
                default:
                    throw new ArgumentException($"Type {treeNode.GetType().Name} is not supported");
            }
        }

        private static Field[] SelectObject(ObjectTreeNode obj)
        {
            return obj
                .Properties
                .Select(p => new Field
                {
                    Name = p.ResponseName,
                    Fields = SelectFields(p.Tree)
                }).ToArray();
        }
    }
}