namespace SemanticRelationsResolver.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Sentence : Element
    {
        public Sentence()
        {
            Words = new List<Word>();
        }

        public ICollection<Word> Words { get; set; }
    }
}