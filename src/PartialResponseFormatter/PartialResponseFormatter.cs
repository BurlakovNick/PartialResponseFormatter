using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PartialResponseFormatter
{
    public class PartialResponseFormatter : IPartialResponseFormatter
    {
        public object Format(object obj, ResponseSpecification responseSpecification)
        {
            if (obj == null)
            {
                return null;
            }

            var treeNode = ObjectTreeTraverser.Traverse(obj.GetType());
            return Traverse(obj, responseSpecification.Fields, treeNode);
        }

        private static object Traverse(object obj, Field[] fields, TreeNode treeNode)
        {
            switch (treeNode)
            {
                case null:
                    return null;
                case EmptyTreeNode _:
                    return obj;
                case ObjectTreeNode objectTree:
                    return TraverseObject(obj, fields, objectTree);
                case CollectionTreeNode collection:
                    var enumerableObj = (IEnumerable)obj;
                    return TraverseEnumerable(enumerableObj, fields, collection.Items);
                case DictionaryTreeNode dictionary:
                    var dictionaryObj = (IDictionary)obj;
                    return TraverseDictionary(dictionaryObj, fields, dictionary.Items);
                default:
                    throw new ArgumentException($"Type {treeNode.GetType().Name} is not supported");
            }
        }

        private static object TraverseObject(object obj, Field[] fields, ObjectTreeNode objectTreeNode)
        {
            if (fields.IsNullOrEmpty())
            {
                return obj;
            }
            
            var result = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                var property = objectTreeNode.FindProperty(field.Name);
                if (property == null)
                {
                    continue;
                }

                var value = property.GetValue(obj);

                var subfields = field.Fields;
                result.Add(property.ResponseName, Traverse(value, subfields, property.Tree));
            }

            return result;
        }

        private static List<object> TraverseEnumerable(IEnumerable enumerableObj, Field[] fields, TreeNode treeNode)
        {
            return enumerableObj
                .Cast<object>()
                .Select(element => Traverse(element, fields, treeNode))
                .Where(formattedElement => formattedElement != null)
                .ToList();
        }

        private static Dictionary<object, object> TraverseDictionary(IDictionary dictionaryObj, Field[] fields, TreeNode treeNode)
        {
            var result = new Dictionary<object, object>();
            foreach (DictionaryEntry kvp in dictionaryObj)
            {
                var formattedElement = Traverse(kvp.Value, fields, treeNode);
                if (formattedElement != null)
                {
                    result.Add(kvp.Key, formattedElement);
                }
            }
            return result;
        }
    }
}