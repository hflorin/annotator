namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Sentence : Element
    {
        public Sentence()
        {
            Words = new List<Word>();
        }

        public ICollection<Word> Words { get; set; }
    }
}