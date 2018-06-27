namespace Treebank.Mappers.Serialization.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Sentence
    {
        [XmlElement("word")]
        public List<Word> Words { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("parser")]
        public string Parser { get; set; }
        [XmlAttribute("user")]
        public string User { get; set; }
        [XmlAttribute("date")]
        public string Date { get; set; }
        [XmlAttribute("citation-part")]
        public string CitationPart { get; set; }
    }
}