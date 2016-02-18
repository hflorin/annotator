namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Document : ModelBase
    {
        public Document()
        {
            Sentences = new List<Sentence>();
        }

        public ICollection<Sentence> Sentences { get; set; }
    }
}