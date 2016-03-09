namespace SemanticRelationsResolver.Domain
{
    using System;
    using System.Collections.Generic;

    public class Document : ModelBase
    {
        public Document()
        {
            Sentences = new List<Sentence>();
        }

        public Document(dynamic content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content", "Must provide document content.");
            }

            DynamicContent = content;
            Sentences = new List<Sentence>();

            Initialize();
        }
        
        public ICollection<Sentence> Sentences { get; set; }

        protected virtual void Initialize()
        {
            Id = DynamicContent.treebank.id;

            foreach (var sentence in DynamicContent.treebank.sentence)
            {
                var words = new List<Word>();

                foreach (var word in sentence.word)
                {
                    words.Add(
                        new Word
                            {
                                Id = word.id,
                                Chunk = word.chunk,
                                Content = word.form,
                                DependencyRelation = word.head == "0" ? string.Empty : word.deprel,
                                Form = word.form,
                                HeadWordId = word.head,
                                Lemma = word.lemma,
                                PartOfSpeech = word.postag
                            });
                }

                Sentences.Add(
                    new Sentence
                        {
                            Date = DateTime.Parse(sentence.date),
                            Id = sentence.id,
                            User = sentence.user,
                            Parser = sentence.parser,
                            Words = words
                        });
            }
        }
    }
}