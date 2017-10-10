namespace Treebank.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Sentence : Element
    {
        public Sentence()
        {
            Words = new List<Word>();
            Metadata = new List<string>();
        }

        public bool IsTree { get; set; }

        public ICollection<Word> Words { get; set; }

        public ICollection<string> Metadata { get; set; }
    }
}