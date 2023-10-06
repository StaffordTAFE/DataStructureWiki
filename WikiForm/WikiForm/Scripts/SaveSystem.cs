using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Diagnostics;

namespace WikiForm
{
    internal class SaveSystem
    {
        Byte[] data;

        public string[,] Load(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        data = binaryReader.ReadBytes(int.Parse(binaryReader.BaseStream.Length.ToString()));
                    }
                }
            }

            return DeserializeData(data);


            // Old load function

            /*Byte[] byteData = File.ReadAllBytes(filePath);

            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream(byteData))
            {
                return bf.Deserialize(ms) as string[,];
            }*/
        }// load file

        Byte[] SerializeData(string[,] data)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }// convert to binary

        string[,] DeserializeData(Byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream(data))
            {
                return bf.Deserialize(ms) as string[,];
            }
        }// convert to binary
    }
}
