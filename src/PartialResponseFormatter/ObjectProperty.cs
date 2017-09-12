using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PartialResponseFormatter
{
    internal class ObjectProperty
    {
        private readonly Func<object, object> propertyGetter;

        public ObjectProperty(PropertyInfo property, TreeNode tree)
        {
            ResponseName = ReflectionProvider.GetPropertyResponseName(property);
            Tree = tree;
            propertyGetter = DelegateGenerator.GeneratePropertyGetter(property);
        }

        public string ResponseName { get; }

        public TreeNode Tree { get; }

        public object GetValue(object obj)
        {
            return propertyGetter(obj);
        }
    }
}