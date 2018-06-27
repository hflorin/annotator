namespace Treebank.Mappers.Serialization.Models
{
    using System.Xml.Serialization;

    public class Word
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("form")]
        public string Form { get; set; }
        [XmlAttribute("lemma")]
        public string Lemma { get; set; }
        [XmlAttribute("postag")]
        public string Postag { get; set; }
        [XmlAttribute("head")]
        public string Head { get; set; }
        [XmlAttribute("chunk")]
        public string Chunk { get; set; }
        [XmlAttribute("deprel")]
        public string DepRel { get; set; }
    }
}