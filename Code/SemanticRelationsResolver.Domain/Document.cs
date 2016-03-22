namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Document : ModelBase
    {
        public Document()
        {
            Sentences = new List<Sentence>();
        }

        public string Identifier { get; set; }

        public ICollection<Sentence> Sentences { get; set; }

        public string FilePath { get; set; }
    }
}