using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huffman_coding
{
    public class Decoder
    {
        public Dictionary<string, char> chartable { get; set; }
        public Decoder(string file_path , string output_path)
        {
            chartable = new Dictionary<string, char>();

            using (FileStream filestream = new (file_path , FileMode.Open))
            using (BinaryReader reader = new (filestream , Encoding.UTF8))
            {
                ReadCharTableFromFile(reader);

                string str= ReadBinaryData(reader);
                string result = DecodBinaryString(str);

                // write decoded data
                File.WriteAllText(output_path, result);

            }
        }
        public void ReadCharTableFromFile (BinaryReader redear)
        {
            int table_count = redear.ReadInt32();
            for(int i=0; i<table_count; i++)
            {
                char key = redear.ReadChar();  
                int length = redear.ReadInt32();  
                string value = Encoding.UTF8.GetString(redear.ReadBytes(length));

                // NOTE: reversed the key value pair for faster search
                chartable[value] = key;
            }
        }

        public string ReadBinaryData (BinaryReader redear)
        {
            //converts it to a binary string representation
            //and removes any padding that might have been added during the encoding process.

            int binarydata_size = redear.ReadInt32();

            int padding_length = redear.ReadInt32();

            //Reads an array of bytes from the binary file
            byte[] arr = redear.ReadBytes(binarydata_size);

            StringBuilder str = new(arr.Length * 8);

            foreach (var v in arr)
                str.Append(Convert.ToString(v, 2).PadLeft(8, '0'));

            //remove padding zeros 
            str.Length -= padding_length;

            return str.ToString();

        }

        private string DecodBinaryString (string str)
        {
            StringBuilder result = new();
            StringBuilder current = new();

            for(int i =0; i<str.Length; i++)
            {
                current.Append(str[i]);

                if(chartable.ContainsKey(current.ToString()))
                {
                    result.Append(chartable[current.ToString()]);
                    current.Clear();
                }
            }

            return result.ToString();
        }

    }
}
