using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanExercise
{
    class HuffmanTree
    {
        private const byte L_BIT = 0;
        private const byte R_BIT = 1;

        public List<HuffmanNode> NodesInTree = new List<HuffmanNode>();
        public HuffmanNode RootNode = null;

        public HuffmanTree(List<HuffmanNode> sortedNodeList)
        {
            List<HuffmanNode> nodesOperating = new List<HuffmanNode>();

            // Copy all pointers to a new list to do the operation
            foreach (HuffmanNode node in sortedNodeList)
            {
                nodesOperating.Add(node);
                NodesInTree.Add(node);
            }

            // Construct the tree from the list
            while (nodesOperating.Count > 2)
            {
                // Create a new parent node of two smallest node
                HuffmanNode newParentNode = new HuffmanNode(nodesOperating[0], nodesOperating[1]);
                NodesInTree.Add(newParentNode);
                // Add the new node into the operating list
                nodesOperating.Add(newParentNode);
                // Remove first two node from list
                nodesOperating.RemoveRange(0, 2);
                // Sort the list
                nodesOperating.Sort((lNode, rNode) => lNode.Weight.CompareTo(rNode.Weight));
            }
            // Create root node
            RootNode = new HuffmanNode(nodesOperating[0], nodesOperating[1]);
            NodesInTree.Add(RootNode);
        }

        /// <summary>
        /// Compress data using generated tree, write into target array
        /// and fill zeros to the tail to complete the data to full bytes if needed
        /// </summary>
        /// <param name="originalData">Data needs to compress</param>
        /// <param name="compressedDataList">List to save byte data</param>
        /// <returns>Count of zeros filled at the tail</returns>
        public byte CompressData(byte[] originalData, List<byte> compressedDataList)
        {
            // Get a map of 'byte to bit string'
            Dictionary<byte, string> dataMap = GetDataMapOfTree();

            byte buffer = 0;        // Data buffer for writing complete byte data
            int needTailZeroCount = 8;  // Count of zeros should be added at tail to complete a byte's data

            LogUtil.Log("Translating original bytes into compressed string...");
            foreach (byte b in originalData)
            {
                string dataString = dataMap[b];

                foreach (char c in dataString)
                {
                    buffer <<= 1;   // Move buffer left a bit
                    (buffer) |= (c == 'L' ? L_BIT : R_BIT);   // Do 'or' calculation for new bit;
                    needTailZeroCount--;

                    // If buffer is full, write the data to the list then clear buffer
                    if (needTailZeroCount == 0)
                    {
                        compressedDataList.Add(buffer);
                        buffer = 0;
                        needTailZeroCount = 8; 
                    }
                }
            }

            if (needTailZeroCount != 8)
            {
                buffer <<= needTailZeroCount; // Add zeros to tail
                compressedDataList.Add(buffer);
            }

            return (byte)(needTailZeroCount % 8);
        }

        public byte[] DecompressData(byte[] compressedData, int tailZeroCount)
        {
            LogUtil.Log("Decompressing data...");
            List<byte> originalData = new List<byte>();
            int remainBitsInByte = 8;
            long operatingByteIndex = 0;
            byte operatingByte = compressedData[0];
            HuffmanNode currentNode = RootNode;

            while (true)
            {
                // Check if reached the end of data
                if (operatingByteIndex == compressedData.LongLength - 1 &&
                    remainBitsInByte == tailZeroCount % 8)
                {
                    return originalData.ToArray();
                }

                // Get the highest bit of current byte as part of path
                currentNode = (operatingByte >> 7) == L_BIT ? currentNode.LeftChild : currentNode.RightChild;

                // If leaf node is found, get original byte of string and reset current node
                if (currentNode.IsLeafNode)
                {
                    originalData.Add(currentNode.NodeValue);
                    currentNode = RootNode;
                }

                // Check if need to move to next bit
                remainBitsInByte--;
                if (remainBitsInByte == 0)
                {
                    remainBitsInByte = 8;
                    operatingByteIndex++;
                    //Console.WriteLine(compressedData.LongLength - operatingByteIndex);
                    if (operatingByteIndex == compressedData.LongLength)
                    {
                        return originalData.ToArray();
                    }
                    operatingByte = compressedData[operatingByteIndex];
                }
                else
                {
                    operatingByte <<= 1;
                }
            }
        }

        /// <summary>
        /// Get the 'data to bit string' map of the tree (DFS)
        /// </summary>
        /// <returns></returns>
        private Dictionary<byte, string> GetDataMapOfTree()
        {
            LogUtil.Log("Arranging 'byte to string' map...");
            Dictionary<byte, string> map = new Dictionary<byte, string>();
            GetPathOfNodeAndChildren(RootNode, "", map);
            return map;
        }

        /// <summary>
        /// Visit all nodes recursively and save the path of leaf node to path dictionary
        /// </summary>
        /// <param name="node">Next node</param>
        /// <param name="nextString">String for next node</param>
        /// <param name="pathDictionary">Dictionary for saving all strings</param>
        private void GetPathOfNodeAndChildren(HuffmanNode node, string nextString,
            Dictionary<byte, string> pathDictionary)
        {
            if (node.IsLeafNode)
            {
                // Save current string to dict
                pathDictionary.Add(node.NodeValue, nextString);
            }

            if (node.LeftChild != null)
            {
                // If has left child, visit it
                GetPathOfNodeAndChildren(node.LeftChild, nextString + "L", pathDictionary);
            }

            if (node.RightChild != null)
            {
                // If has right child, visit it
                GetPathOfNodeAndChildren(node.RightChild, nextString + "R", pathDictionary);
            }
        }
    }
}
