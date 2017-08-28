using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialResponseFormatter
{
    public static class ResponseSpecificationMatcher
    {
        private static readonly FieldMismatch[] Empty = new FieldMismatch[0];
        private static readonly Dictionary<string, ObjectProperty> EmptyProperties = new Dictionary<string, ObjectProperty>();

        public static FieldMismatch[] FindClientFieldMismatches<TClientData, TServerData>()
        {
            var clientTree = ObjectTreeTraverser.Traverse<TClientData>();
            var serverTree = ObjectTreeTraverser.Traverse<TServerData>();
            return FindClientFieldMismatches(clientTree, serverTree, "root").ToArray();
        }

        public static FieldMismatch[] FindClientFieldMismatches<TServerData>(ResponseSpecification clientResponseSpecification)
        {
            var serverTree = ObjectTreeTraverser.Traverse<TServerData>();
            return FindClientFieldMismatches(clientResponseSpecification.Fields, serverTree, "root").ToArray();
        }

        private static IEnumerable<FieldMismatch> FindClientFieldMismatches(
            TreeNode clientTree,
            TreeNode serverTree,
            string path
        )
        {
            if (clientTree.Type != serverTree.Type)
            {
                return CreateTypeMismatch(clientTree, serverTree, path);
            }

            switch (serverTree.Type)
            {
                case NodeType.Empty:
                    return Empty;
                case NodeType.Object:
                    return FindObjectMismatches(clientTree, serverTree, path);
                case NodeType.Collection:
                    return FindCollectionMismatches(clientTree, serverTree, path);
                case NodeType.Dictionary:
                    return FindDictionaryMismatches(clientTree, serverTree, path);
                default:
                    throw new ArgumentOutOfRangeException($"Node type {serverTree.GetType().Name} is not supported");
            }
        }

        private static IEnumerable<FieldMismatch> CreateTypeMismatch(
            TreeNode clientTree,
            TreeNode serverTree,
            string path
        )
        {
            var message = $"Different node type. Client = {clientTree.Type}, server = {serverTree.Type}.";
            yield return new FieldMismatch(path, message);
        }

        private static IEnumerable<FieldMismatch> FindObjectMismatches(
            TreeNode clientTree,
            TreeNode serverTree,
            string path
        )
        {
            var clientObject = (ObjectTreeNode) clientTree;
            var serverObject = (ObjectTreeNode) serverTree;
            var serverPropertiesMap = serverObject.Properties.ToDictionary(p => p.ResponseName);
            foreach (var clientProperty in clientObject.Properties)
            {
                var propertyPath = $"{path}.{clientProperty.ResponseName}";
                if (serverPropertiesMap.TryGetValue(clientProperty.ResponseName, out var serverProperty))
                {
                    var mismatches = FindClientFieldMismatches(clientProperty.Tree, serverProperty.Tree, propertyPath);
                    foreach (var mismatch in mismatches)
                    {
                        yield return mismatch;
                    }
                }
                else
                {
                    yield return new FieldMismatch(propertyPath, "Client has field, but server has not.");
                }
            }
        }

        private static IEnumerable<FieldMismatch> FindCollectionMismatches(
            TreeNode clientTree,
            TreeNode serverTree,
            string path
        )
        {
            var clientCollection = (CollectionTreeNode) clientTree;
            var serverCollection = (CollectionTreeNode) serverTree;
            return FindClientFieldMismatches(clientCollection.Items, serverCollection.Items, $"{path}.[list]");
        }

        private static IEnumerable<FieldMismatch> FindDictionaryMismatches(
            TreeNode clientTree,
            TreeNode serverTree,
            string path
        )
        {
            var clientDictionary = (DictionaryTreeNode) clientTree;
            var serverDictionary = (DictionaryTreeNode) serverTree;
            return FindClientFieldMismatches(clientDictionary.Items, serverDictionary.Items, $"{path}.[dictionary]");
        }

        private static IEnumerable<FieldMismatch> FindClientFieldMismatches(
            Field[] clientFields,
            TreeNode serverTree,
            string path
        )
        {
            if (clientFields.IsNullOrEmpty())
            {
                return Empty;
            }
            
            switch (serverTree.Type)
            {
                case NodeType.Empty:
                    return FindObjectMismatches(clientFields, path, EmptyProperties);
                case NodeType.Object:
                    var serverObject = (ObjectTreeNode) serverTree;
                    var serverPropertiesMap = serverObject.Properties.ToDictionary(p => p.ResponseName);
                    return FindObjectMismatches(clientFields, path, serverPropertiesMap);
                case NodeType.Collection:
                    return FindCollectionMismatches(clientFields, serverTree, path);
                case NodeType.Dictionary:
                    return FindDictionaryMismatches(clientFields, serverTree, path);
                default:
                    throw new ArgumentOutOfRangeException($"Node type {serverTree.GetType().Name} is not supported");
            }
        }

        private static IEnumerable<FieldMismatch> FindObjectMismatches(
            Field[] clientFields,
            string path,
            Dictionary<string, ObjectProperty> serverPropertiesMap
            )
        {
            foreach (var clientField in clientFields)
            {
                var propertyPath = $"{path}.{clientField.Name}";
                if (serverPropertiesMap.TryGetValue(clientField.Name, out var serverProperty))
                {
                    var mismatches = FindClientFieldMismatches(clientField.Fields, serverProperty.Tree, propertyPath);
                    foreach (var mismatch in mismatches)
                    {
                        yield return mismatch;
                    }
                }
                else
                {
                    yield return new FieldMismatch(propertyPath, "Client has field, but server has not.");
                }
            }
        }

        private static IEnumerable<FieldMismatch> FindCollectionMismatches(
            Field[] clientFields,
            TreeNode serverTree,
            string path
        )
        {
            var serverCollection = (CollectionTreeNode) serverTree;
            return FindClientFieldMismatches(clientFields, serverCollection.Items, $"{path}.[list]");
        }

        private static IEnumerable<FieldMismatch> FindDictionaryMismatches(
            Field[] clientFields,
            TreeNode serverTree,
            string path
        )
        {
            var serverDictionary = (DictionaryTreeNode) serverTree;
            return FindClientFieldMismatches(clientFields, serverDictionary.Items, $"{path}.[dictionary]");
        }
    }
}