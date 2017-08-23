using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialResponseFormatter
{
    internal class ObjectTreeNode : TreeNode
    {
        private readonly Dictionary<string, ObjectProperty> propertiesMap;

        public ObjectTreeNode(ObjectProperty[] properties)
        {
            var duplicateProperties = properties
                .GroupBy(x => x.ResponseName)
                .Where(x => x.Count() > 1)
                .ToArray();
                        
            if (duplicateProperties.Any())
            {
                var duplicates = string.Join(";", duplicateProperties.Select(p => p.Key));
                throw new InvalidOperationException($"Duplicate properties: {duplicates}");
            }

            propertiesMap = properties.ToDictionary(p => p.ResponseName);
            Properties = properties;
        }

        public ObjectProperty[] Properties { get; }

        public ObjectProperty FindProperty(string fieldName) =>
            propertiesMap.TryGetValue(fieldName, out var result) ? result : null;

        public override NodeType Type => NodeType.Object;
    }
}