using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuffmanCompression
{
    #region Enums
    public enum Relative : int
    {
        leftChild,
        rightChild,
        parent,
        root
    };
    #endregion

    public class BinaryTree<E>
    {
        #region Properties
        private StringBuilder Encoding;
        public int Size { get; private set; }
        public BinaryTreeNode<E> Current { get; set; }
        public BinaryTreeNode<E> Root { get; set; }
        #endregion

        #region Constructors
        public BinaryTree()
        {
            Root = null;
            Current = null;
            Size = 0;
            Encoding = new StringBuilder();
        }

        public BinaryTree(E data)
        {
            Root = new BinaryTreeNode<E>(data);
            Current = null;
            Size = 0;
            Encoding = new StringBuilder();
        }
        #endregion

        #region Public Methods
        public void Destroy(BinaryTreeNode<E> node)
        {
            if (node != null)
            {
                Destroy(node.Left);
                Destroy(node.Right);
                node = null;
                Size--;
            }
        }

        public bool isEmpty()
        {
            return Root == null;
        }

        /// <summary>
        /// Starter code for the creation of the encoding table.  To be completed. 
        /// Hint!  Use the debugger to trace the execution and determine what action(s)
        /// to be performed
        /// </summary>
        /// <param name="p"></param>
        public void BuildEncodingTable(BinaryTreeNode<E> p)
        {
            var chs = new List<CharacterEncoding>();
            if (p != null)
            {
                Encoding.Append("0");
                BuildEncodingTable(p.Left);

                if (p.IsLeaf())
                {
                    Console.WriteLine(p.Data.ToString());
                    Encoding.Append("_");
                    Console.WriteLine(Encoding);
                    
                }

                Encoding.Append("1");
                BuildEncodingTable(p.Right);
            }
            else
            {
                // Remove a character from the encoding string// Check and reove multiple 0's too maybe?
                Encoding.Remove(Encoding.Length - 1, 1);
                //Console.WriteLine("remove!");
            }
        }
        public string GetEncoding()
        {
            return Encoding.ToString();
        }
        //public void TestEncodingTable()
        //{
        //    foreach (var item in CharEncodings)
        //    {
        //        var temp = item as CharacterEncoding;
        //        if (temp != null)
        //        {
        //            Console.WriteLine(temp.Character.ToString() + " - " + temp.Encoding.ToString());
        //        }
        //    }
        //    Console.ReadLine();
        //}

        public void preOrder(BinaryTreeNode<E> p)
        {
            if (p != null)
            {
                Console.WriteLine(p.Data.ToString());
                preOrder(p.Left);
                preOrder(p.Right);
            }
        }

        public void postOrder(BinaryTreeNode<E> p)
        {
            if (p != null)
            {
                postOrder(p.Left);
                postOrder(p.Right);
                Console.WriteLine(p.Data.ToString());
            }
        }

        public void inOrder(BinaryTreeNode<E> p)
        {
            if (p != null)
            {
                inOrder(p.Left);
                Console.WriteLine(p.Data.ToString());
                inOrder(p.Right);
            }
        }

        private BinaryTreeNode<E> findParent(BinaryTreeNode<E> n)
        {
            Stack<BinaryTreeNode<E>> s = new Stack<BinaryTreeNode<E>>();
            n = Root;
            while (n.Left != Current && n.Right != Current)
            {
                if (n.Right != null)
                    s.Push(n.Right);

                if (n.Left != null)
                    n = n.Left;
                else
                    n = s.Pop();
            }
            s.Clear();
            return n;
        }

        public bool Insert(BinaryTreeNode<E> node, Relative rel)
        {
            bool inserted = true;

            if ((rel == Relative.leftChild && Current.Left != null)
                    || (rel == Relative.rightChild && Current.Right != null))
            {
                inserted = false;
            }
            else
            {
                switch (rel)
                {
                    case Relative.leftChild:
                        Current.Left = node;
                        break;
                    case Relative.rightChild:
                        Current.Right = node;
                        break;
                    case Relative.root:
                        if (Root == null)
                            Root = node;
                        Current = Root;
                        break;
                }
                Size++;
            }
            return inserted;
        }

        public bool Insert(E data, Relative rel)
        {
            bool inserted = true;

            BinaryTreeNode<E> node = new BinaryTreeNode<E>(data);

            if ((rel == Relative.leftChild && Current.Left != null)
                    || (rel == Relative.rightChild && Current.Right != null))
            {
                inserted = false;
            }
            else
            {
                switch (rel)
                {
                    case Relative.leftChild:
                        Current.Left = node;
                        break;
                    case Relative.rightChild:
                        Current.Right = node;
                        break;
                    case Relative.root:
                        if (Root == null)
                            Root = node;
                        Current = Root;
                        break;
                }
                Size++;
            }
            return inserted;
        }
        public bool moveTo(Relative rel)
        {
            bool found = false;

            switch (rel)
            {
                case Relative.leftChild:
                    if (Current.Left != null)
                    {
                        Current = Current.Left;
                        found = true;
                    }
                    break;
                case Relative.rightChild:
                    if (Current.Right != null)
                    {
                        Current = Current.Right;
                        found = true;
                    }
                    break;
                case Relative.parent:
                    if (Current != Root)
                    {
                        Current = findParent(Current);
                        found = true;
                    }
                    break;
                case Relative.root:
                    if (Root != null)
                    {
                        Current = Root;
                        found = true;
                    }
                    break;
            } // end Switch relative
            return found;
        }
        #endregion
    }
}
