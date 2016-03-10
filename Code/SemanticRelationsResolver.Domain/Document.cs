namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;
    using Prism.Events;

    public class Document : ModelBase
    {
        public Document()
        {
            Sentences = new List<Sentence>();
        }

        public IEventAggregator EventAggregator { get; set; }

        public string Identifier { get; set; }

        public ICollection<Sentence> Sentences { get; set; }

        public string FilePath { get; set; }
    }
}