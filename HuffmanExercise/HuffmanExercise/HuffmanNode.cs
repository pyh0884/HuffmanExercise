using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanExercise
{
    class HuffmanNode
    {
        public bool IsLeafNode = true;
        public byte NodeValue = byte.MinValue;
        public int Weight = 0;

        public HuffmanNode LeftChild = null;
        public HuffmanNode RightChild = null;
        public HuffmanNode ParentNode = null;

        /// <summary>
        /// Create a new end node stores a value
        /// </summary>
        /// <param name="val">Node's value</param>
        /// <param name="weight">Weight of this node</param>
        public HuffmanNode(byte val, int weight)
        {
            NodeValue = val;
            Weight = weight;
        }

        /// <summary>
        /// Create a parent node with two children
        /// </summary>
        /// <param name="leftChild"></param>
        /// <param name="rightChild"></param>
        public HuffmanNode(HuffmanNode leftChild, HuffmanNode rightChild)
        {
            IsLeafNode = false;
            LeftChild = leftChild;
            RightChild = rightChild;

            leftChild.ParentNode = ParentNode;
            rightChild.ParentNode = ParentNode;
            Weight = leftChild.Weight + rightChild.Weight;
        }

        public override string ToString()
        {
            return IsLeafNode ? ("Leaf v:" + NodeValue + " w:" + Weight) : ("NotLeaf w:" + Weight);
        }
    }
}
