using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace SkJukeBox_Demo.Utility
{
    public static class Serializer<T> where T : class
    {
        private static DataContractSerializer _dataContractSerializer = new DataContractSerializer(typeof(T));

        public static string Serialize(T @object)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                Serialize(@object, stringWriter);
            }
            return sb.ToString();
        }

        public static void SerializeToFile(T @object, string filepath)
        {
            using (var stringWriter = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                Serialize(@object, stringWriter);
            }
        }

        private static void Serialize(T @object, TextWriter stringWriter)
        {
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                xmlWriter.Formatting = Formatting.Indented;
                _dataContractSerializer.WriteObject(xmlWriter, @object);
            }
        }


        public static T Deserialize(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                return Deserialize(stringReader);
            }
        }

        public static T DeserializeFromFile(string filepath)
        {
            using (var streamReader = new StreamReader(filepath))
            {
                return Deserialize(streamReader);
            }
        }

        private static T Deserialize(TextReader stringReader)
        {
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                return _dataContractSerializer.ReadObject(xmlReader) as T;
            }
        }

        public static XmlReader GetXmlReader(T @object)
        {
            string xml = Serialize(@object);
            return new XmlTextReader(new StringReader(xml));
        }

    }
}
