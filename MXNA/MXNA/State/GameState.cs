using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;


namespace MXNA
{
    //[Serializable]
    public class GameState
    {
        Dictionary<string, object> data;

        public GameState()
        {
            data = new Dictionary<string, object>();
        }

        public void Set<T>(string key, T value)
        {

            if (!data.ContainsKey(key))
                data.Add(key, value);
            else
                data[key] = value;
        }

        public T Get<T>(string key)
        {
            return (data.ContainsKey(key) ? (T)data[key] : default(T));
        }

        public void Save()
        {
            //string filename = "item.xml";
            //string path = Path.Combine(StorageContainer.TitleLocation, filename);

            //using (XmlWriter writer = XmlWriter.Create(path))
            //{
            //    //XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string, object>));
            //    //serializer.Serialize(writer, data);
            //    //IntermediateSerializer.Serialize<Dictionary<string, object>>(writer, data, filename);
            //}
        }

        public void Load()
        {
            //string filename = "item.xml";
            //string path = Path.Combine(StorageContainer.TitleLocation, filename);

            //using (XmlReader reader = XmlReader.Create(path))
            //{
            //    //XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string, object>));
            //    //data = serializer.Deserialize(reader) as Dictionary<string, object>;
            //    //data = IntermediateSerializer.Deserialize<Dictionary<string, object>>(reader, filename);
            //}
        }
    }

}