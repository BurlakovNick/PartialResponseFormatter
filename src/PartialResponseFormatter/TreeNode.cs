namespace PartialResponseFormatter
{
    internal abstract class TreeNode
    {
        public abstract NodeType Type { get; }
        public static TreeNode Empty => new EmptyTreeNode();
        public static TreeNode Object(ObjectProperty[] properties) => new ObjectTreeNode(properties);
        public static TreeNode Collection(TreeNode items) => new CollectionTreeNode(items);
        public static TreeNode Dictionary(TreeNode items) => new DictionaryTreeNode(items);
    }
}