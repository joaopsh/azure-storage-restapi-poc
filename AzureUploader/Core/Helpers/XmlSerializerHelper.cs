using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AzureUploader.Core.Helpers
{
    public static class XmlSerializerHelper
    {
        public static string Serialize<T>(T obj)
        {
            var xml = string.Empty;
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                {
                    xsSubmit.Serialize(xmlWriter, obj);
                    xml = stringWriter.ToString();
                }
            }

            return xml;
        } 
    }
}
