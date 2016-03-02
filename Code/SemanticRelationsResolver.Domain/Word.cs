namespace SemanticRelationsResolver.Domain
{
    public class Word : ModelBase
    {
        public string Form { get; set; }

        public string Lemma { get; set; }

        public string PartOfSpeech { get; set; }

        public string HeadWordId { get; set; }

        public string Chunk { get; set; }

        public string DependencyRelation { get; set; }
    }
}