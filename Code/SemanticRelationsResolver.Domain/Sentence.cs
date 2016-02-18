namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Sentence
    {
        public Sentence()
        {
            Words = new List<Word>();
        }

        public ICollection<Word> Words { get; private set; }

        public void AddWord(Word word)
        {
            if (word == null || Words.Contains(word))
            {
                return;
            }

            Words.Add(word);
        }
    }
}