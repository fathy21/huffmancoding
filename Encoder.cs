using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huffman_coding
{
    public class Encoder
    {
        public Dictionary<char, string> CharTable { get; set; }

        public Encoder(string input, string filePath) 
        {
            CharTable = new Dictionary<char, string>();

            Node root = BuildHuffmanTree(input);
            BuildCharTable(root, "");
            WriteToFile(filePath, input);
        }

        private Node BuildHuffmanTree (string input )
        {
            PriorityQueue<Node, int> pq = new();

            // Count the frequency of each character in the input string
            var frequencies = input.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            
            foreach(var kvp in frequencies) 
                pq.Enqueue(new Node { symbol = kvp.Key, frequency = kvp.Value },kvp.Value);
            
            while(pq.Count > 1)
            {
                Node left =pq.Dequeue();   

                Node right =pq.Dequeue();

                // combine the two children's frequencies
                int cominedfreq = left.frequency + right.frequency;

                Node parent_node = new() { symbol = '\0', frequency = cominedfreq, Left = left, Right = right };

                pq.Enqueue(parent_node, cominedfreq);
            }
            return pq.Peek();
        }

        private void BuildCharTable(Node node , string pre)
        {
            if (node == null)
                return;

            if (!CharTable.ContainsKey(node.symbol))
                CharTable.Add(node.symbol, pre);

            if(node.Left!=null)
                BuildCharTable(node.Left, pre + "0");

            if (node.Right != null)
                BuildCharTable(node.Right, pre + "1");
        }

        private string GenerateBinaryString (string input)
        {
            StringBuilder str = new();

            for(int i=0; i<input.Length; i++)
            {
                str.Append(CharTable[input[i]]);
            }

            return str.ToString();  
        }
        private void WriteCharTable (BinaryWriter writer)
        {
            writer.Write(CharTable.Count());

            foreach(var v in CharTable)
            {
                char symbol = v.Key;
                string code = v.Value;

                writer.Write(symbol);
                writer.Write(code.Length);
                writer.Write(Encoding.UTF8.GetBytes(v.Value));
            }
        }

        private void WriteBinaryStringToFile(BinaryWriter writer, string str)
        {
            // get padding size and pad right
            int padding_Length = (8 - (str.Length % 8)) % 8;
            str = str.PadRight(str.Length + padding_Length, '0');

            // write data size and padding length
            writer.Write(str.Length / 8);
            writer.Write(padding_Length);

            for (int i = 0; i < str.Length; i += 8)
            {
                string eightBits = str.Substring(i, Math.Min(8, str.Length - i));
                writer.Write(Convert.ToByte(eightBits, 2));
            }
        }

        private void WriteToFile(string filePath, string input)
        {
            string str = GenerateBinaryString(input);


            using (BinaryWriter writer = new(new FileStream(filePath, FileMode.Create)))
            {
                WriteCharTable(writer);
                WriteBinaryStringToFile(writer, str);
            }
        }
    }
}
