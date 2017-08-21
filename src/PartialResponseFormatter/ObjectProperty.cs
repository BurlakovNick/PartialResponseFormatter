using System.Reflection;

namespace PartialResponseFormatter
{
    internal class ObjectProperty
    {
        private readonly PropertyInfo property;

        public ObjectProperty(PropertyInfo property, TreeNode tree)
        {
            this.property = property;
            ResponseName = ReflectionProvider.GetPropertyResponseName(property);
            Tree = tree;
        }

        public string ClrName => property.Name;
        public string ResponseName { get; }

        //todo: some better name
        public TreeNode Tree { get; set; }

        public object GetValue(object obj)
        {
            return property.GetValue(obj);
        }
    }
}