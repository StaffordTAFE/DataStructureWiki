using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace WikiForm
{
    internal class SaveSystem
    {
        readonly string path = Application.UserAppDataPath; // path to save to

        public void Save(string[,] data, string filePath)
        {
            if (!File.Exists(filePath)) // if file doesnt exist, create it
            {
                var newFile = File.Create(filePath);
                newFile.Close();
            }

            Byte[] byteData = SerializeData(data);

            File.WriteAllBytes(filePath, byteData);
        }// save file

        public string[,] Load(string filePath)
        {
            Byte[] byteData = File.ReadAllBytes(filePath);

            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream(byteData))
            {
                return bf.Deserialize(ms) as string[,];
            }
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
    }
}
