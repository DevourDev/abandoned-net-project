using System.IO;
using System.Xml.Serialization;

namespace DevourEncoding.Serialization.Xml
{
    public static class DevourXml
    {
        public static void SaveXmlToFile<T>(string savePath, string fileName, T dataToSave)
        {
            XmlSerializer xs = new(typeof(T));
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            using FileStream fs = new(Path.Combine(savePath,fileName), FileMode.Create);
            xs.Serialize(fs, dataToSave);
        }

        public static byte[] Serialize<T>(T dataToSerialize)
        {
            XmlSerializer xs = new(typeof(T));
            MemoryStream ms = new();
            xs.Serialize(ms, dataToSerialize);
            return ms.ToArray();
        }

        public static T Deserialize<T>(byte[] data)
        {
            XmlSerializer xs = new(typeof(T));
            MemoryStream ms = new(data);
            return (T)xs.Deserialize(ms);
        }

        public static T LoadXmlFile<T>(string filePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using FileStream fs = new FileStream(filePath, FileMode.Open);
            return (T)xs.Deserialize(fs);
        }


    }

}
