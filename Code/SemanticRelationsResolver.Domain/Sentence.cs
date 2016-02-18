namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Sentence : ModelBase
    {
        public Sentence()
        {
            Words = new List<Word>();
        }

        public ICollection<Word> Words { get; set; }
    }
}