namespace Treebank.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration;
    using Domain;
    using Events;
    using Loaders;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class DocumentMapperWithDynamic : IDocumentMapper
    {
        public IResourceLoader ResourceLoader { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public IAppConfigMapper AppConfigMapper { get; set; }

        public async Task<Document> Map(string filepath, string configFilepath)
        {
            var documentContent = await ResourceLoader.LoadAsync(filepath).ConfigureAwait(false);

            var appConfig = await AppConfigMapper.Map(configFilepath);

            var document = CreateDocument(documentContent, appConfig);

            document.FilePath = filepath;

            return document;
        }

        private Document CreateDocument(dynamic documentContent, IAppConfig appConfig)
        {
            var datastructure =
                appConfig.DataStructures.FirstOrDefault(d => d.Format == ConfigurationStaticData.XmlFormat);

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish("Could not load XML file because the structure is not defined in the configuration file.");
                return null;
            }

            var documentElementPrototype = datastructure.Elements.OfType<Document>().Single();
            var sentenceElementPrototype = datastructure.Elements.OfType<Sentence>().Single();
            var wordElementPrototype = datastructure.Elements.OfType<Word>().Single();

            var documentElement = ObjectCopier.Clone(documentElementPrototype);

            documentElement.Attributes.Clear();

            var documentIdAttribute =
                ObjectCopier.Clone(documentElementPrototype.Attributes.Single(a => a.Name.Equals("id")));

            documentIdAttribute.Value = documentContent.treebank.id;
            documentElement.Attributes.Add(documentIdAttribute);

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
                        EventAggregator.GetEvent<ValidationExceptionEvent>()
                            .Publish(string.Format("[Exception] Invalid word id: {0}", word.id));
                        continue;
                    }

                    int headWordId;
                    if (!int.TryParse(word.head, out headWordId))
                    {
                        EventAggregator.GetEvent<ValidationExceptionEvent>()
                            .Publish(string.Format("[Exception] Invalid head word id: {0}", word.head));
                        continue;
                    }

                    var newWord = ObjectCopier.Clone(wordElementPrototype);
                    newWord.Attributes.Clear();

                    var wordIdAttribute =
                        ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("id")));
                    wordIdAttribute.Value = word.id;

                    var wordFormAttribute =
                        ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("form")));
                    wordFormAttribute.Value = word.form;

                    var wordLemmaAttribute =
                        ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("lemma")));
                    wordLemmaAttribute.Value = word.lemma;

                    var wordPostagAttribute =
                        ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("postag")));
                    wordPostagAttribute.Value = word.postag;

                    var wordHeadAttribute =
                        ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("head")));
                    wordHeadAttribute.Value = word.head;

                    var wordChunkAttribute =
                        ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("chunk")));
                    wordChunkAttribute.Value = word.chunk;

                    newWord.Attributes.Add(wordIdAttribute);
                    newWord.Attributes.Add(wordFormAttribute);
                    newWord.Attributes.Add(wordLemmaAttribute);
                    newWord.Attributes.Add(wordPostagAttribute);
                    newWord.Attributes.Add(wordHeadAttribute);
                    newWord.Attributes.Add(wordChunkAttribute);
                    if (wordHeadAttribute.Value != "0")
                    {
                        var wordDeprelAttribute =
                            ObjectCopier.Clone(wordElementPrototype.Attributes.Single(a => a.Name.Equals("deprel")));

                        wordDeprelAttribute.Value = word.deprel;
                        newWord.Attributes.Add(wordDeprelAttribute);
                    }

                    newWord.Attributes.Add(
                        new Attribute {Name = "content", DisplayName = "Content", Value = word.form});

                    words.Add(newWord);
                }

                int sentenceId;
                if (!int.TryParse(sentence.id, out sentenceId))
                {
                    EventAggregator.GetEvent<ValidationExceptionEvent>()
                        .Publish(string.Format("[Exception] Invalid sentence id: {0}", sentence.id));
                    continue;
                }

                DateTime date;
                if (!DateTime.TryParse(sentence.date, out date))
                {
                    EventAggregator.GetEvent<ValidationExceptionEvent>()
                        .Publish(string.Format("[Exception] Invalid sentence date: {0}", sentence.date));
                    continue;
                }

                var newSentence = ObjectCopier.Clone(sentenceElementPrototype);
                newSentence.Attributes.Clear();

                var sentenceIdAttribute =
                    ObjectCopier.Clone(sentenceElementPrototype.Attributes.Single(a => a.Name.Equals("id")));
                sentenceIdAttribute.Value = sentence.id;
                newSentence.Attributes.Add(sentenceIdAttribute);

                var sentenceParserAttribute =
                    ObjectCopier.Clone(sentenceElementPrototype.Attributes.Single(a => a.Name.Equals("parser")));
                sentenceParserAttribute.Value = sentence.parser;
                newSentence.Attributes.Add(sentenceParserAttribute);

                var sentenceUserAttribute =
                    ObjectCopier.Clone(sentenceElementPrototype.Attributes.Single(a => a.Name.Equals("user")));
                sentenceUserAttribute.Value = sentence.user;
                newSentence.Attributes.Add(sentenceUserAttribute);

                var sentenceDateAttribute =
                    ObjectCopier.Clone(sentenceElementPrototype.Attributes.Single(a => a.Name.Equals("date")));
                sentenceDateAttribute.Value = sentence.date;
                newSentence.Attributes.Add(sentenceDateAttribute);

                newSentence.Attributes.Add(
                    new Attribute
                    {
                        Name = "content",
                        DisplayName = "Content",
                        Value = sentenceBody.ToString(0, sentenceBody.Length - 1)
                    });

                newSentence.Words = words;

                documentElement.Sentences.Add(newSentence);
            }

            return documentElement;
        }
    }
}