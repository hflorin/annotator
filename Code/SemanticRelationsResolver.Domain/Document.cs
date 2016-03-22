namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Document : Element
    {
        public Document()
        {
            Sentences = new List<Sentence>();
        }

        public ICollection<Sentence> Sentences { get; set; }

        public string FilePath { get; set; }
    }
}