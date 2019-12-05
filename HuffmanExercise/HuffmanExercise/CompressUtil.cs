using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanExercise
{
    class CompressUtil
    {
        // Compressed data structure:
        // 1. A byte for count of zeros added at tail of data
        // 2. 2 bytes for length of frequency list
        // 3. Frequency list (<part2> * <cell length>). Cell length = 1(original data) * 4(  
        // 4. Compressed data

        /// <summary>
        /// Compress a file
        /// </summary>
        /// <param name="original">File will be compressed</param>
        public static byte[] Compress(FileInfo original)
        {
            LogUtil.Log("Starting compress. File size:" + original.Length);

            LogUtil.Log("Loading file to memory...");
            byte[] originalData = File.ReadAllBytes(original.FullName);

            LogUtil.Log("Generating frequency list...");
            List<HuffmanNode> sortedList = CreateByteFrequencyListFromOriginalFile(originalData);

            LogUtil.Log("Generating Huffman Tree...");
            HuffmanTree tree = new HuffmanTree(sortedList);

            List<byte> compressedDataList = new List<byte>();

            // Compress data and get the count of zeros
            byte tailZeroCount = tree.CompressData(originalData, compressedDataList);
            LogUtil.Log(tailZeroCount + " zeros added at the end");

            // Save frequency list's length to 2 bytes
            byte[] frequencyListLength = new byte[2];
            frequencyListLength[1] = (byte) (sortedList.Count);
            frequencyListLength[0] = (byte) (sortedList.Count >> 8);

            // Save frequency list
            LogUtil.Log("Saving frequency list...");
            List<byte> frequencyList = new List<byte>();
            foreach (HuffmanNode node in sortedList)
            {
                List<byte> listUnit = new List<byte>();
                listUnit.Add(node.NodeValue);
                listUnit.Add((byte)((node.Weight & 0xff000000) >> 24));
                listUnit.Add((byte)((node.Weight & 0x00ff0000) >> 16));
                listUnit.Add((byte)((node.Weight & 0x0000ff00) >> 8));
                listUnit.Add((byte)((node.Weight & 0x000000ff)));
                frequencyList.AddRange(listUnit);
            }

            List<byte> fullData = new List<byte>();
            fullData.Add(tailZeroCount);
            fullData.AddRange(frequencyListLength);
            fullData.AddRange(frequencyList);
            fullData.AddRange(compressedDataList);

            return fullData.ToArray();
        }

        /// <summary>
        /// Decompress a file
        /// </summary>
        /// <param name="compressed">Compressed FileInfo</param>
        public static byte[] Decompress(FileInfo compressed)
        {
            LogUtil.Log("Starting decompress.");

            LogUtil.Log("Loading file to memory...");
            byte[] fullData = File.ReadAllBytes(compressed.FullName);

            int tailZeroCount = fullData[0];
            int frequencyListLength = (fullData[1] << 8) | fullData[2];
            List<byte> frequencyListData = new List<byte>();

            LogUtil.Log("Loading frequency list data...");
            // Frequency list starts from the 3rd byte
            // Each element in frequency list takes 5 byte to save (1 byte for original data and 4 bytes for frequency)
            for (int i = 3; i < 3 + frequencyListLength * 5; i++)
            {
                frequencyListData.Add(fullData[i]);
            }

            LogUtil.Log("Loading compressed data...");
            // Compressed data starts after frequency list
            List<byte> compressedData = new List<byte>();
            for (int i = 3 + 5 * frequencyListLength; i < fullData.Length; i++)
            {
                compressedData.Add(fullData[i]);
            }

            LogUtil.Log("Reconstructing Huffman tree with loaded frequency list...");
            // Reconstruct Huffman tree
            List<HuffmanNode> frequencyList = CreateByteFrequencyListFromCompressedByteList(frequencyListData);
             HuffmanTree tree = new HuffmanTree(frequencyList);

            byte[] originalData = tree.DecompressData(compressedData.ToArray(), tailZeroCount);

            return originalData;
        }

        /// <summary>
        /// Construct a sorted byte frequency list for constructing a Huffman tree 
        /// </summary>
        /// <param name="data">Original data</param>
        /// <returns>Sorted list of frequency</returns>
        public static List<HuffmanNode> CreateByteFrequencyListFromOriginalFile(byte[] data)
        {
            // Count all bytes appeared in the array
            Dictionary<byte, int> byteCountDict = new Dictionary<byte, int>();
            foreach (byte b in data)
            {
                if (!byteCountDict.ContainsKey(b))
                {
                    byteCountDict.Add(b, 0);
                }

                byteCountDict[b]++;
            }

            // Turn dictionary into a list and sort it
            List<HuffmanNode> frequencyList = new List<HuffmanNode>();
            foreach (KeyValuePair<byte, int> pair in byteCountDict)
            {
                frequencyList.Add(new HuffmanNode(pair.Key, pair.Value));
            }

            // Sort in ascending order
            frequencyList.Sort((lNode, rNode) => lNode.Weight.CompareTo(rNode.Weight));

            return frequencyList;
        }

        /// <summary>
        /// Construct a sorted byte frequency list for constructing a Huffman tree 
        /// </summary>
        /// <param name="data">Compressed byte frequency list</param>
        /// <param name="elementCount">Count of elements in list</param>
        /// <param name="frequncyLength">Length of frequency count in byte</param>
        /// <returns></returns>
        public static List<HuffmanNode> CreateByteFrequencyListFromCompressedByteList(List<byte> data)
        {
            List<HuffmanNode> frequencyList = new List<HuffmanNode>();

            for (int i = 0; i < data.Count; i += 5)
            {
                int frequencyOfNode =
                    (data[i + 1] << 24) |
                    (data[i + 2] << 16) |
                    (data[i + 3] << 8) |
                    (data[i + 4]);

                frequencyList.Add(new HuffmanNode(data[i], frequencyOfNode));
            }

            //frequencyList.Sort((lNode, rNode) => lNode.Weight.CompareTo(rNode.Weight));
            return frequencyList;
        }
    }
}
