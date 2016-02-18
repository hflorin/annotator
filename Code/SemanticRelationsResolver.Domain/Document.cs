namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public class Document : ModelBase
    {
        public Document()
        {
            Sentences = new List<Sentence>();
        }

        public ICollection<Sentence> Sentences { get; private set; }

        public void AddSentence(Sentence sentence)
        {
            if (sentence == null || Sentences.Contains(sentence))
            {
                return;
            }

            Sentences.Add(sentence);
        }
    }
}