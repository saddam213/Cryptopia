using System;
using System.IO;
using System.Xml.Serialization;

namespace Cryptopia.API.Utils
{
	public static class Serialization
    {
        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="filename">The filename.</param>
        public static void Serialize<T>(T obj, string filename)
        {
            //Create our own namespaces for the output
            var ns = new XmlSerializerNamespaces();
            ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

            var mySerializer = new XmlSerializer(typeof(T));
            using (var myWriter = new StreamWriter(filename))
            {
                mySerializer.Serialize(myWriter, obj, ns);
            }
        }

        /// <summary>
        /// Deserializes the specified object from a file.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns>The object from the file</returns>
        public static T Deserialize<T>(string filename)
        {
            var mySerializer = new XmlSerializer(typeof(T));
            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var obj = (T)mySerializer.Deserialize(fileStream);
                return obj;
            }
        }

        /// <summary>
        /// Creates a copy of a serailizable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static T CreateCopy<T>(this T obj)
        {
            //Create our own namespaces for the output
            var ns = new XmlSerializerNamespaces();
            ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

            var mySerializer = new XmlSerializer(typeof(T));
            using (var myWriter = new MemoryStream())
            {
                mySerializer.Serialize(myWriter, obj, ns);
                myWriter.Seek(0, SeekOrigin.Begin);
                var copy = (T)mySerializer.Deserialize(myWriter);

                return copy;
            }
        }

        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <typeparam name="T">the obnject to serialize</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string SerializeToString<T>(T obj)
        {
            try
            {
                //Create our own namespaces for the output
                var ns = new XmlSerializerNamespaces();
                ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

                var mySerializer = new XmlSerializer(typeof(T));
                using (var myWriter = new StringWriter())
                {
                    mySerializer.Serialize(myWriter, obj, ns);
                    return myWriter.ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T">the object type to deserialize</typeparam>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string xmlString)
        {
            try
            {
                var mySerializer = new XmlSerializer(typeof(T));
                using (var stringReader = new StringReader(xmlString))
                {
                    return (T)mySerializer.Deserialize(stringReader);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
