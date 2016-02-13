using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sitter.ToolBox.Utility
{
    public class ObjectSerializerHelper
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();

        private static XmlSerializer GetSerializer(Type t)
        {
            if (!_serializers.ContainsKey(t))
            {
                _serializers.Add(t, new XmlSerializer(t) {  });
            }

            return _serializers[t];
        }

        public static void SerializeObjectToXmlFile(object serializableObject, string fileName) // futuredev: rename to SerializeObjectToFile
        {
            if (serializableObject == null)
            {
                return;
            }

            // STEP - Remove read-only flag if it exists
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists && fileInfo.IsReadOnly)
                fileInfo.IsReadOnly = false;

            lock (_serializers)
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    XmlWriter writer = XmlWriter.Create(fs, new XmlWriterSettings() {Indent = false});
                    XmlSerializer xmlSerializer = GetSerializer(serializableObject.GetType());
                    xmlSerializer.Serialize(writer, serializableObject);
                }
            }
        }

        public static T DeSerializeObjectFromFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return default(T);
            }

            T objectOut;

            lock (_serializers)
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xmlSerializer = GetSerializer(typeof (T));
                    objectOut = (T) xmlSerializer.Deserialize(fs);
                }
            }

            return objectOut;
        }
    }
}