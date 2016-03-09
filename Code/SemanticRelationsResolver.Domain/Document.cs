namespace SemanticRelationsResolver.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Events;
    using Prism.Events;

    public class Document : ModelBase
    {
        public IEventAggregator EventAggregator { get; set; }

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

        public string Identifier { get; set; }

        public ICollection<Sentence> Sentences { get; set; }

        protected virtual void Initialize()
        {
            Identifier = DynamicContent.treebank.id;

            foreach (var sentence in DynamicContent.treebank.sentence)
            {
                var words = new List<Word>();
                var sentenceBody = new StringBuilder();

                foreach (var word in sentence.word)
                {
                    sentenceBody.Append(string.Format("{0} ", word.form));

                    int wordId;
                    if (!int.TryParse(word.id,out wordId))
                    {
                        EventAggregator.GetEvent<DocumentLoadExceptionEvent>().Publish(string.Format("[Exception] Invalid word id: {0}", word.id));
                        continue;
                    }

                    int headWordId;
                    if (!int.TryParse(word.head, out headWordId))
                    {
                        EventAggregator.GetEvent<DocumentLoadExceptionEvent>().Publish(string.Format("[Exception] Invalid head word id: {0}", word.head));
                        continue;
                    }

                    words.Add(
                        new Word
                        {
                            Id = wordId,
                            Chunk = word.chunk,
                            Content = word.form,
                            DependencyRelation = headWordId == 0 ? string.Empty : word.deprel,
                            Form = word.form,
                            HeadWordId = headWordId,
                            Lemma = word.lemma,
                            PartOfSpeech = word.postag
                        });
                }

                int sentenceId;
                if (!int.TryParse(sentence.id, out sentenceId))
                {
                    EventAggregator.GetEvent<DocumentLoadExceptionEvent>().Publish(string.Format("[Exception] Invalid sentence id: {0}", sentence.id));
                    continue;
                }

                DateTime date;
                if (!DateTime.TryParse(sentence.date, out date))
                {
                    EventAggregator.GetEvent<DocumentLoadExceptionEvent>().Publish(string.Format("[Exception] Invalid sentence date: {0}", sentence.date));
                    continue;
                }

                Sentences.Add(
                    new Sentence
                    {
                        Date = date,
                        Id = sentenceId,
                        User = sentence.user,
                        Parser = sentence.parser,
                        Content = sentenceBody.ToString(0, sentenceBody.Length - 1),
                        Words = words
                    });
            }
        }
    }
}