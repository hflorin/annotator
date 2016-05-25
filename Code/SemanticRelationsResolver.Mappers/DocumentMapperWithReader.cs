namespace SemanticRelationsResolver.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Prism.Events;

    using SemanticRelationsResolver.Domain;
    using SemanticRelationsResolver.Domain.Configuration;
    using SemanticRelationsResolver.Events;

    using Attribute = SemanticRelationsResolver.Domain.Attribute;

    public class DocumentMapperWithReader : IDocumentMapper
    {
        private Document documentPrototype;

        private Sentence sentencePrototype;

        private Word wordPrototype;

        public IEventAggregator EventAggregator { get; set; }

        public IAppConfigMapper AppConfigMapper { get; set; }

        public async Task<Document> Map(string filepath, string configFilepath)
        {
            var appConfig = await AppConfigMapper.Map(configFilepath);

            if (appConfig == null)
            {
                throw new ArgumentNullException(
                    "configFilepath", 
                    string.Format("Could not load configuration file from: {0}", configFilepath));
            }

            wordPrototype = appConfig.Elements.OfType<Word>().Single();
            sentencePrototype = appConfig.Elements.OfType<Sentence>().Single();
            documentPrototype = appConfig.Elements.OfType<Document>().Single();

            var document = CreateDocument(filepath, appConfig);

            document.FilePath = filepath;

            return document;
        }

        private Document CreateDocument(string filepath, IAppConfig appConfig)
        {
            var document = ObjectCopier.Clone(documentPrototype);

            var reader = new XmlTextReader(filepath);

            var queue = new List<ConfigurationPair>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        var pair = new ConfigurationPair { ElementName = reader.Name };

                        var entityAttributes = new Dictionary<string, string>();

                        while (reader.MoveToNextAttribute())
                        {
                            entityAttributes.Add(reader.Name, reader.Value);
                        }

                        pair.Attributes.Add(entityAttributes);
                        queue.Add(pair);
                        break;
                    case XmlNodeType.EndElement:
                        AddElementsToDocument(document, queue, appConfig);
                        break;
                }
            }

            AddInternalAttributes(document);

            return document;
        }

        private void AddInternalAttributes(Document document)
        {
            foreach (var sentence in document.Sentences)
            {
                var sentenceContent = string.Empty;
                foreach (var word in sentence.Words)
                {
                    var formAttribute = word.Attributes.SingleOrDefault(a => a.Name.Equals("form"));
                    var formValue = formAttribute != null ? formAttribute.Value : string.Empty;

                    sentenceContent += " " + formValue;
                    word.Attributes.Add(new Attribute { Name = "content", DisplayName = "Content", Value = formValue });
                }

                sentence.Attributes.Add(
                    new Attribute { Name = "content", DisplayName = "Content", Value = sentenceContent });
            }
        }

        private void AddElementsToDocument(
            Document document, 
            ICollection<ConfigurationPair> queue, 
            IAppConfig appConfig)
        {
            if (document == null)
            {
                return;
            }

            if (!queue.Any())
            {
                return;
            }

            foreach (var item in queue)
            {
                var entityName = appConfig.Elements.Single(e => e.Name.Equals(item.ElementName)).Entity;

                if (string.IsNullOrWhiteSpace(entityName))
                {
                    break;
                }

                var entity = EntityFactory.GetEntity(entityName);

                if (entity is Word)
                {
                    var newWord = ObjectCopier.Clone(wordPrototype);
                    newWord.Attributes.Clear();

                    AddAttributesToElement(item, wordPrototype, newWord);
                    NotifyIfAnyNonOptionalAttributeIsMissing(document, wordPrototype, newWord);

                    var lastSentence = document.Sentences.Last();
                    lastSentence.Words.Add(newWord);
                }
                else if (entity is Sentence)
                {
                    var newSentence = ObjectCopier.Clone(sentencePrototype);
                    newSentence.Attributes.Clear();

                    AddAttributesToElement(item, sentencePrototype, newSentence);
                    NotifyIfAnyNonOptionalAttributeIsMissing(document, sentencePrototype, newSentence);

                    document.Sentences.Add(newSentence);
                }
                else if (entity is Document)
                {
                    document.Attributes.Clear();

                    AddAttributesToElement(item, documentPrototype, document);
                    NotifyIfAnyNonOptionalAttributeIsMissing(document, documentPrototype, document);
                }
                else
                {
                    throw new UnknownEntityTypeException(string.Format("Unkown entity {0}", item.ElementName));
                }
            }

            queue.Clear();
        }

        private void AddAttributesToElement(ConfigurationPair item, Element elementPrototype, Element elementToModify)
        {
            foreach (var itemAttributes in item.Attributes)
            {
                foreach (var itemAttribute in itemAttributes)
                {
                    var attributeAdded = false;

                    foreach (var wordPrototypeAttribute in elementPrototype.Attributes)
                    {
                        if (itemAttribute.Key.Equals(wordPrototypeAttribute.Name))
                        {
                            var prototypeAttributeCopy = ObjectCopier.Clone(wordPrototypeAttribute);

                            prototypeAttributeCopy.Value = itemAttribute.Value;

                            elementToModify.Attributes.Add(prototypeAttributeCopy);
                            attributeAdded = true;
                            break;
                        }
                    }

                    if (!attributeAdded)
                    {
                        elementToModify.Attributes.Add(
                            new Attribute
                                {
                                    Name = itemAttribute.Key, 
                                    DisplayName = itemAttribute.Key, 
                                    Value = itemAttribute.Value, 
                                    IsOptional = true, 
                                    IsEditable = true
                                });
                    }
                }
            }
        }

        private void NotifyIfAnyNonOptionalAttributeIsMissing(
            Document document, 
            Element elementPrototype, 
            Element newElement)
        {
            const string MissingNonOptionalAttributeErrorMessage =
                "Value missing for: attribute {0}, word id {1}, sentence id: {2}, document id: {3}";

            foreach (var wordPrototypeAttribute in elementPrototype.Attributes)
            {
                if (!wordPrototypeAttribute.IsOptional)
                {
                    var wordAttribute =
                        newElement.Attributes.SingleOrDefault(atr => atr.Name.Equals(wordPrototypeAttribute.Name));
                    if (wordAttribute == null || string.IsNullOrEmpty(wordAttribute.Value))
                    {
                        var lastSentence = document.Sentences.Last();
                        var newWordId = newElement.Attributes.Single(a => a.Name.Equals("id")).Value;
                        var sentenceId = lastSentence.Attributes.Single(a => a.Name.Equals("id")).Value;
                        var documentId = document.Attributes.Single(a => a.Name.Equals("id")).Value;

                        EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                            .Publish(
                                string.Format(
                                    MissingNonOptionalAttributeErrorMessage, 
                                    wordPrototypeAttribute.Name, 
                                    newWordId, 
                                    sentenceId, 
                                    documentId));
                    }
                }
            }
        }
    }
}