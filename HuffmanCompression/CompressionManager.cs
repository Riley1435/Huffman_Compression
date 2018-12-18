using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HuffmanCompression
{
    public class CompressionManager
    {
        #region Constants
        private const byte END_OF_FILE_KEY = 47; // backslash "/"
        #endregion

        #region Properties
        public string FileRead { get; set; }
        public string FileWrite { get; set; }
        public CharacterFrequency[] CharFrequencies { get; set; }
        public BinaryTree<CharacterFrequency> HuffmanTree { get; set; }
        public CharacterEncoding[] CharEncodings { get; set; }
        public StringBuilder Encoding { get; set; }
        public Dictionary<string, byte> EncodingDict { get; set; }
        public long ByteOffset { get; set; }
        #endregion

        #region Constructors
        public CompressionManager()
        {
            HuffmanTree = new BinaryTree<CharacterFrequency>();
            Encoding = new StringBuilder();
            EncodingDict = new Dictionary<string, byte>();
            ByteOffset = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Main execution method for this class
        /// Could be structured better but I wanted
        /// to easily show the Big O notations for each
        /// step of the compression.
        /// </summary>
        public void Compress()
        {
            FileReader();
            SortCharFrequencies(); //O(n^2) worst case 
            DeleteEmptyCharsFromArray(); //O(n)
            BuildHuffmanTree();
            BuildEncodingTable(HuffmanTree.Root);
            FileWriterEncoding();
            FileWriterCompression();

            Console.ReadLine();
        }
        public void CompressTest()
        {
            FileReader();
            SortCharFrequencies(); //O(n^2) worst case 
            DeleteEmptyCharsFromArray(); //O(n)
            BuildHuffmanTree();
            DisplayEncodingTable(HuffmanTree.Root);
            //FileWriterEncoding();
            //FileWriterCompression();
            Console.ReadLine();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Decompress()
        {
            BuildEncodingDictionary();
            DecompressFile();
        }
        /// <summary>
        /// Read file into CharacterFrequency[] property
        /// </summary>
        public void FileReader()
        {
            StringBuilder path = new StringBuilder();

            path.Append(Directory.GetCurrentDirectory());
            path.Append(@"\" + FileRead);

            FileInfo file = new FileInfo(path.ToString());
            try
            {
                using (StreamReader reader = file.OpenText())
                {
                    byte ch;
                    for (int i = 1; i <= file.Length; i++)
                    {
                        ch = (byte)reader.Read();
                        CharFrequencies[ch].Increment();
                    }
                }
            }
            catch
            {
                Console.WriteLine("Could not read File");
            }
        }

        /// <summary>
        /// Writes CharEncodings to file. For Decompression using RegEx.
        /// </summary>
        public void FileWriterEncoding()
        {
            StringBuilder path = new StringBuilder();
            path.Append(Directory.GetCurrentDirectory());
            path.Append(@"\" + FileWrite + ".txt");

            try
            {
                using (StreamWriter writer = new StreamWriter(path.ToString()))
                {
                    CharacterEncoding.WriteEncoding(writer, CharEncodings);
                }
                Console.WriteLine($"File {FileWrite}.txt has been written");
            }
            catch
            {
                Console.WriteLine("Could not write File");
            }
        }

        /// <summary>
        /// Writes input file to new file according to the Huffman tree encoding
        /// </summary>
        public void FileWriterCompression()
        {
            StringBuilder writePath = new StringBuilder();
            writePath.Append(Directory.GetCurrentDirectory());
            writePath.Append(@"\" + FileWrite + ".txt");

            StringBuilder readPath = new StringBuilder();
            readPath.Append(Directory.GetCurrentDirectory());
            readPath.Append(@"\" + FileRead);

            FileInfo file = new FileInfo(readPath.ToString());
            try
            {
                using (StreamReader reader = file.OpenText())
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(writePath.ToString(), FileMode.Append)))
                    {
                        //TODO: Check and make sure this writes the byte in the desired order.
                        byte wByte = 0;
                        int exponent = 0;
                        while (!reader.EndOfStream) 
                        {
                            byte rByte = (byte)reader.Read();
                            string enc = CharEncodings[rByte].Encoding;
                            while (enc.Length > 0)
                            {
                                byte bitLoc = (byte)Math.Pow(2, exponent);

                                if (enc.Substring(0, 1) == "1")
                                {
                                    wByte = (byte)(wByte | bitLoc);
                                }

                                enc = enc.Remove(0, 1);
                                exponent++;

                                if (exponent == 8)
                                {
                                    byte temp = wByte;
                                    writer.Write(temp);
                                    wByte = 0;
                                    exponent = 0;
                                }
                            }
                        }
                        Console.WriteLine("File is done compressing.");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Could not compress file.");
            }
        }

        public void SortCharFrequencies()
        {
            Array.Sort(CharFrequencies);
        }

        public void DeleteEmptyCharsFromArray()
        {
            CharFrequencies = CharFrequencies.Where(x => x.Frequency != 0)
                .ToArray();
        }

        public void BuildHuffmanTree()
        {
            List<BinaryTreeNode<CharacterFrequency>> nodeList = ToTreeNodes();

            while (nodeList.Count > 1)
            {
                var lowest = nodeList[0];
                var secondLowest = nodeList[1];
                var tempNode = new BinaryTreeNode<CharacterFrequency>(new CharacterFrequency());

                //sum lowest 2 nodes into new node
                tempNode.Data.Frequency = lowest.Data.Frequency + secondLowest.Data.Frequency;
                tempNode.Left = lowest;
                tempNode.Right = secondLowest;

                //removes the first 2 elements from the list;
                nodeList.RemoveAt(0);
                nodeList.RemoveAt(0);

                int index = FindIndexToInsert(nodeList, tempNode.Data.Frequency);

                nodeList.Insert(index, tempNode);
            }

            HuffmanTree.Root = nodeList[0];
        }

        /// <summary>
        /// Returns the compressed file until the end of the ecoding table.
        /// </summary>
        /// <returns></returns>
        public string GetRawEncodingTable()
        {
            StringBuilder compressedFile = new StringBuilder();
            compressedFile.Append(Directory.GetCurrentDirectory());
            compressedFile.Append(@"\" + FileRead);

            FileInfo file = new FileInfo(compressedFile.ToString());
            var rawEncTable = new StringBuilder();
            bool foundEnd = false;

            using (StreamReader reader = file.OpenText())
            {
                while (!foundEnd)
                {
                    byte temp = (byte)reader.Read();
                    if (temp == END_OF_FILE_KEY)
                        foundEnd = true;
                    else
                        rawEncTable.Append((char)temp);
                }
            }
            ByteOffset = rawEncTable.Length + 1;
            return rawEncTable.ToString();
        }

        /// <summary>
        /// Parses the encodings from the raw encoding string into a Hash Table/Dict
        /// </summary>
        public void BuildEncodingDictionary()
        {
            string rawEncTable = GetRawEncodingTable();

            string pattern = @"(\d{1,3})([_])([0-1]*)([_]{2})?";

            Match match = Regex.Match(rawEncTable, pattern);
            while (match.Success)
            {
                byte ch = (byte)Convert.ToInt32(match.Groups[1].Value);
                EncodingDict.Add(match.Groups[3].Value, ch);

                match = match.NextMatch();
            }
        }

        public void DecompressFile()
        {
            StringBuilder writePath = new StringBuilder();
            writePath.Append(Directory.GetCurrentDirectory());
            writePath.Append(@"\" + FileWrite + ".txt");

            StringBuilder readPath = new StringBuilder();
            readPath.Append(Directory.GetCurrentDirectory());
            readPath.Append(@"\" + FileRead);

            FileInfo file = new FileInfo(readPath.ToString());

            Console.WriteLine("File is decompressing");

            using (StreamWriter writer = new StreamWriter(writePath.ToString()))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(readPath.ToString(), FileMode.Open)))
                {
                    reader.BaseStream.Seek(ByteOffset, SeekOrigin.Begin);
                    bool found = false;
                    int exponent = 0;
                    StringBuilder encoding = new StringBuilder();
                    byte rByte = reader.ReadByte();

                    while (reader.BaseStream.Position != (reader.BaseStream.Length + ByteOffset))
                    {
                        while (!found)
                        {
                            //byte bitLoc = (byte)Math.Pow(2, exponent);
                            if (IsBitOn(rByte, exponent))
                                encoding.Append("1");
                            else
                                encoding.Append("0");

                            byte val;
                            if (EncodingDict.TryGetValue(encoding.ToString(), out val))
                            {

                                writer.Write((char)val);
                                encoding.Clear();
                                exponent++;
                                found = true;
                            }
                            else
                            {
                                exponent++;
                            }

                            if (exponent > 7)
                            {
                                if (reader.BaseStream.Position < reader.BaseStream.Length)
                                {
                                    rByte = reader.ReadByte();
                                    exponent = 0;
                                }
                                else
                                {
                                    Console.WriteLine("File is done Compressing");
                                    Console.ReadKey();
                                    Environment.Exit(0);
                                }
                            }
                        }
                        found = false;
                    }
                }
            }
        }

        private bool IsBitOn(byte b, int bitLoc)
         {
            return (b & (1 << bitLoc)) != 0;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Linear search to find index to insert from the given list.
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        private int FindIndexToInsert(List<BinaryTreeNode<CharacterFrequency>> nodeList,
            int search)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (search < nodeList[i].Data.Frequency)
                {
                    return i;
                }
            }
            return nodeList.Count;
        }

        /// <summary>
        /// Turns all character frequencies that have value into a BTNode
        /// </summary>
        /// <returns></returns>
        private List<BinaryTreeNode<CharacterFrequency>> ToTreeNodes()
        {
            List<BinaryTreeNode<CharacterFrequency>> nodeList =
                new List<BinaryTreeNode<CharacterFrequency>>();
            foreach (var cf in CharFrequencies)
            {
                nodeList.Add(new BinaryTreeNode<CharacterFrequency>(cf));
            }
            return nodeList;
        }

        /// <summary>
        /// Builds Encoding table for the given huffman tree root node
        /// </summary>
        /// <param name="root">Huffamn tree root</param>
        private void BuildEncodingTable(BinaryTreeNode<CharacterFrequency> root)
        {
            if (root != null)
            {
                Encoding.Append("0");
                BuildEncodingTable(root.Left);
                if (root.IsLeaf())
                {
                    byte index = (byte)root.Data.Character;
                    CharEncodings[index].Encoding = Encoding.ToString();
                }

                Encoding.Append("1");
                BuildEncodingTable(root.Right);

                if (Encoding.Length > 0)
                {
                    Encoding.Remove(Encoding.Length - 1, 1);
                }
            }
            else
            {
                // Remove a character from the encoding string// Check and reove multiple 0's too maybe?
                Encoding.Remove(Encoding.Length - 1, 1);
                //Console.WriteLine("remove!");
            }
        }
        #endregion

        #region Testing Methods
        /// <summary>
        /// Main TESTING execution method for this class
        /// </summary>
        public void TestCompress()
        {
            SortCharFrequencies();
            DisplayAllCharFrequencies();
            DeleteEmptyCharsFromArray(); //NOTE: describe this operation and complexity O(n); explain the logic.
            BuildHuffmanTree();
            DisplayEncodingTable(HuffmanTree.Root);
        }

        /// <summary>
        /// Displays data in the ChararcterFrequency[] property
        /// </summary>
        public void DisplayAllCharFrequencies()
        {
            foreach (var item in CharFrequencies)
            {
                Console.WriteLine(item.Character + " - " + item.Frequency);
            }
        }

        /// <summary>
        /// Displays All nodes of a Binary Search Tree
        /// </summary>
        /// <param name="root"></param>
        private void DisplayTree(BinaryTreeNode<CharacterFrequency> root)
        {
            if (root == null)
                return;

            DisplayTree(root.Left);
            Console.WriteLine(root.Data.Character + " - " + root.Data.Frequency);
            DisplayTree(root.Right);
        }

        /// <summary>
        /// Method used for testing that displays encoding to the console.
        /// </summary>
        /// <param name="root"></param>
        private void DisplayEncodingTable(BinaryTreeNode<CharacterFrequency> root)
        {
            if (root != null)
            {
                Encoding.Append("0");
                DisplayEncodingTable(root.Left);
                if (root.IsLeaf())
                {
                    var charEncoding = new CharacterEncoding((byte)root.Data.Character, Encoding.ToString());
                    Console.Write(charEncoding.Character.ToString());
                    Console.Write(" - ");
                    Console.WriteLine(charEncoding.Encoding);
                }

                Encoding.Append("1");
                DisplayEncodingTable(root.Right);

                if (Encoding.Length > 0)
                {
                    Encoding.Remove(Encoding.Length - 1, 1);
                }
            }
            else
            {
                // Remove a character from the encoding string// Check and reove multiple 0's too maybe?
                Encoding.Remove(Encoding.Length - 1, 1);
                //Console.WriteLine("remove!");
            }
        }

        public void TestFileWriter()
        {
            StringBuilder path = new StringBuilder();
            path.Append(Directory.GetCurrentDirectory());
            path.Append(@"\" + FileWrite + ".txt");

            try
            {
                using (StreamWriter writer = new StreamWriter(path.ToString()))
                {
                    CharacterFrequency.WriteArray(writer, CharFrequencies);
                }
                Console.WriteLine($"File {FileWrite}.txt has been written");
            }
            catch
            {
                Console.WriteLine("Could not write File");
            }
        }
        #endregion
    }
}
