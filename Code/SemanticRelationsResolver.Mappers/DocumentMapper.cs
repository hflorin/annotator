namespace SemanticRelationsResolver.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Events;
    using Loaders;
    using Prism.Events;

    public class DocumentMapper : IDocumentMapper
    {
        public IResourceLoader ResourceLoader { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public async Task<Document> Map(string filepath)
        {
            var documentContent = await ResourceLoader.LoadAsync(filepath).ConfigureAwait(false);

            var document = CreateDocument(documentContent);

            document.FilePath = filepath;

            return document;
        }

        private Document CreateDocument(dynamic documentContent)
        {
            var document = new Document {Identifier = documentContent.treebank.id};


            foreach (var sentence in documentContent.treebank.sentence)
            {
                var words = new List<Word>();
                var sentenceBody = new StringBuilder();

                foreach (var word in sentence.word)
                {
                    sentenceBody.Append(string.Format("{0} ", word.form));

                    int wordId;
                    if (!int.TryParse(word.id, out wordId))
                    {
                        EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                            .Publish(string.Format("[Exception] Invalid word id: {0}", word.id));
                        continue;
                    }

                    int headWordId;
                    if (!int.TryParse(word.head, out headWordId))
                    {
                        EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                            .Publish(string.Format("[Exception] Invalid head word id: {0}", word.head));
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
                    EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                        .Publish(string.Format("[Exception] Invalid sentence id: {0}", sentence.id));
                    continue;
                }

                DateTime date;
                if (!DateTime.TryParse(sentence.date, out date))
                {
                    EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                        .Publish(string.Format("[Exception] Invalid sentence date: {0}", sentence.date));
                    continue;
                }

                document.Sentences.Add(
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
            return document;
        }
    }
}