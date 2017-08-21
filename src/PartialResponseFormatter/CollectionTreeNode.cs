﻿namespace PartialResponseFormatter
{
    internal class CollectionTreeNode : TreeNode
    {
        public CollectionTreeNode(TreeNode items)
        {
            Items = items;
        }
        
        public TreeNode Items { get; }
    }
}