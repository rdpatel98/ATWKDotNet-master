using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SHABS.COMMON
{
    public class MyXmlSerializer
    {

        public static string Serialize<TType>(TType data)
        {

            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(data.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, data, emptyNamepsaces);
                return stream.ToString();
            }
            // var stringwriter = new System.IO.StringWriter();
            //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(data.GetType());
            //serializer.Serialize(stringwriter, data);
            //return stringwriter.ToString(); ;
        }

        public static TType DeSerialize<TType>(string xmlData)
        {
            var stringReader = new System.IO.StringReader(xmlData);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TType));
            return (TType)serializer.Deserialize(stringReader);
        }
    }
}
