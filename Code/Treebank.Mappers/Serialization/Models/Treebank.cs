namespace Treebank.Mappers.Serialization.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("treebank", IsNullable = false)]
    public class Treebank
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("sentence")]
        public List<Sentence> Sentences { get; set; }
    }
}