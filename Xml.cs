using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MobilityScm.Utilerias
{
    public static class Xml
    {
        public static string ConvertListToXml<T>(List<T> list)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            var documentWriter = new StringWriter();
            var textWriter = new XmlTextWriter(documentWriter) { Formatting = Formatting.Indented };
            serializer.Serialize(textWriter, list);
            return documentWriter.ToString();
        }
    }
}
