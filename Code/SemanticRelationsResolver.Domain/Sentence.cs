namespace SemanticRelationsResolver.Domain
{
    using System;
    using System.Collections.Generic;

    public class Sentence : ModelBase
    {
        public Sentence()
        {
            Words = new List<Word>();
        }

        public string Parser { get; set; }

        public string User { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Word> Words { get; set; }
    }
}