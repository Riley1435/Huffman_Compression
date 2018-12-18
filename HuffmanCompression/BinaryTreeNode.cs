using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuffmanCompression
{
    public class BinaryTreeNode<E>
    {
        #region Properties
        public E Data { get; set; }
        public BinaryTreeNode<E> Left { get; set; }
        public BinaryTreeNode<E> Right { get; set; }
        #endregion

        #region Constructors
        public BinaryTreeNode()
        {

        }
        public BinaryTreeNode(E data)
        {
            Data = data;
            Left = null;
            Right = null;
        }
        #endregion

        #region Public methods
        public bool IsLeaf()
        {
            return (Left == null && Right == null);
        }

        public override bool Equals(object obj)
        {
            return (this == (BinaryTreeNode<E>)obj);
        }
        #endregion
    }
}
