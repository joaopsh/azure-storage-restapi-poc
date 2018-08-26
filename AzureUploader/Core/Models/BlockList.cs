using System.Collections.Generic;
using System.Xml.Serialization;

namespace AzureUploader.Core.Models
{
    [XmlRoot(ElementName = "BlockList")]
    public class BlockList
    {
        [XmlElement(ElementName = "Latest")]
        public List<string> Latest { get; set; }

        [XmlElement(ElementName = "Uncommitted")]
        public List<string> Uncommitted { get; set; }

        [XmlElement(ElementName = "Committed")]
        public List<string> Committed { get; set; }
    }
}
