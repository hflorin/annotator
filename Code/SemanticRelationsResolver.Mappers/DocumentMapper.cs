namespace SemanticRelationsResolver.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Prism.Events;

    using SemanticRelationsResolver.Domain;
    using SemanticRelationsResolver.Events;
    using SemanticRelationsResolver.Loaders;

    using Attribute = SemanticRelationsResolver.Domain.Attribute;

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
            var document = new Document();
            document.Attributes.Add(new Attribute { Name = "id", DisplayName = "Id", Value = documentContent.treebank.id });

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

                    var newWord = new Word();

                    newWord.Attributes.Add(new Attribute { Name = "id", DisplayName = "Id", Value = word.id });
                    newWord.Attributes.Add(new Attribute { Name = "form", DisplayName = "Form", Value = word.form });
                    newWord.Attributes.Add(new Attribute { Name = "lemma", DisplayName = "Lemma", Value = word.lemma });
                    newWord.Attributes.Add(new Attribute { Name = "postag", DisplayName = "Part Of Speech", Value = word.postag });
                    newWord.Attributes.Add(new Attribute { Name = "head", DisplayName = "Head Word Id", Value = word.headWordId });
                    newWord.Attributes.Add(new Attribute { Name = "chunk", DisplayName = "Chunk", Value = word.chunk });
                    newWord.Attributes.Add(new Attribute { Name = "deprel", DisplayName = "Dependency Relation", Value = word.id == "0" ? string.Empty : word.deprel });
                    newWord.Attributes.Add(new Attribute { Name = "content", DisplayName = "Content", Value = word.form });

                    words.Add(newWord);
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

                var newSentence = new Sentence();

                newSentence.Attributes.Add(new Attribute { Name = "id", DisplayName = "Id", Value = sentence.id });
                newSentence.Attributes.Add(new Attribute { Name = "parser", DisplayName = "Parser", Value = sentence.parser });
                newSentence.Attributes.Add(new Attribute { Name = "user", DisplayName = "User", Value = sentence.user });
                newSentence.Attributes.Add(new Attribute { Name = "date", DisplayName = "Date", Value = sentence.date });
                newSentence.Attributes.Add(new Attribute { Name = "content", DisplayName = "Content", Value = sentenceBody.ToString(0, sentenceBody.Length - 1) });

                document.Sentences.Add(newSentence);
            }
            return document;
        }
    }
}