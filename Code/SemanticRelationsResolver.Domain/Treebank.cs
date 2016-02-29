namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;
    using System.Dynamic;

    public class Treebank : DynamicDocument
    {
        public Treebank(ExpandoObject content) : base(content)
        {
        }

        protected override void Initialize()
        {
            var words = new List<Word>();
            foreach (var word in DocumentContent.sentence.word)
            {
                words.Add(new Word
                {
                    Id = word.id,
                    Chunk = word.chunk,
                    Content = word.content,
                    DependencyRelation = word.deprel,
                    Form = word.form,
                    HeadWordId = word.head,
                    Lemma = word.lemma,
                    PartOfSpeech = word.postag
                });
            }

            foreach (var sentence in DocumentContent.sentence)
            {
                Sentences.Add(new Sentence
                {
                    Date = sentence.date,
                    Id = sentence.id,
                    User = sentence.parser,
                    Parser = sentence.parser,
                    Words = words
                });
            }
        }
    }
}