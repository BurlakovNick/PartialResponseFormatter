﻿namespace PartialResponseFormatter
{
    internal class DictionaryTreeNode : TreeNode
    {
        public DictionaryTreeNode(TreeNode items)
        {
            Items = items;
        }
        
        public TreeNode Items { get; }
    }
}