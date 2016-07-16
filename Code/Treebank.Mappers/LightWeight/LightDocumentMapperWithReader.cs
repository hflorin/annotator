﻿namespace Treebank.Mappers.LightWeight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Configuration;
    using Domain;
    using Events;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class LightDocumentMapperWithReader : IDocumentMapper
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

            var datastructure =
                appConfig.DataStructures.FirstOrDefault(d => d.Format == ConfigurationStaticData.XmlFormat);

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish("Could not load XML file because the structure is not defined in the configuration file.");
                return null;
            }

            wordPrototype = datastructure.Elements.OfType<Word>().Single();
            sentencePrototype = datastructure.Elements.OfType<Sentence>().Single();
            documentPrototype = datastructure.Elements.OfType<Document>().Single();

            var document = await CreateDocument(filepath, datastructure);

            document.FilePath = filepath;

            var filenameToPathMapping = AppConfig.GetConfigFileNameToFilePathMapping();

            document.Attributes.Add(
                new Attribute
                {
                    AllowedValuesSet = filenameToPathMapping.Values,
                    Value = appConfig.Name,
                    Name = "configuration",
                    DisplayName = "Configuration",
                    Entity = "attribute",
                    IsEditable = true,
                    IsOptional = false
                });

            document.Attributes.Add(
                new Attribute
                {
                    Value = appConfig.Filepath,
                    Name = "configurationFilePath",
                    DisplayName = "Configuration file path",
                    Entity = "attribute",
                    IsEditable = false,
                    IsOptional = false
                });

            return document;
        }

        private static string GetEntityNameByElementName(DataStructure dataStructure, ConfigurationPair item)
        {
            return dataStructure.Elements.Single(e => e.Name.Equals(item.ElementName)).Entity;
        }

        private async Task<Document> CreateDocument(string filepath, DataStructure dataStructure)
        {
            var document = ObjectCopier.Clone(documentPrototype);

            await ParseDocument(filepath, dataStructure, document);

            await Task.Run(() => AddInternalAttributes(document));

            return document;
        }

        private async Task ParseDocument(string filepath, DataStructure dataStructure, Document document)
        {
            using (var reader = new XmlTextReader(filepath))
            {
                var queue = new List<ConfigurationPair>();

                while (await Task.Run(() => reader.Read()))
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element :
                            var pair = new ConfigurationPair {ElementName = reader.Name};

                            var entityAttributes = new Dictionary<string, string>();

                            while (reader.MoveToNextAttribute())
                            {
                                entityAttributes.Add(reader.Name, reader.Value);
                            }

                            pair.Attributes.Add(entityAttributes);
                            queue.Add(pair);
                            break;
                        case XmlNodeType.EndElement :
                            AddElementsToDocument(document, queue, dataStructure);
                            break;
                    }
                }
            }
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
                    word.Attributes.Add(
                        new Attribute
                        {
                            Name = "content",
                            DisplayName = "Content",
                            Value = formValue,
                            IsOptional = true,
                            IsEditable = false
                        });
                }

                sentence.Attributes.Add(
                    new Attribute
                    {
                        Name = "content",
                        DisplayName = "Content",
                        Value = sentenceContent,
                        IsOptional = true,
                        IsEditable = false
                    });
            }
        }

        private void AddElementsToDocument(
            Document document,
            ICollection<ConfigurationPair> queue,
            DataStructure dataStructure)
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
                var entityName = GetEntityNameByElementName(dataStructure, item);

                if (string.IsNullOrWhiteSpace(entityName))
                {
                    break;
                }

                var entity = EntityFactory.GetEntity(entityName);

                if (entity is Word)
                {
                    ParseWord(document, item);
                }
                else if (entity is Sentence)
                {
                    ParseSentence(document, item);
                }
                else if (entity is Document)
                {
                    ParseDocument(document, item);
                }
                else
                {
                    throw new UnknownEntityTypeException(string.Format("Unkown entity {0}", item.ElementName));
                }
            }

            queue.Clear();
        }

        private void ParseDocument(Document document, ConfigurationPair item)
        {
            AddAttributesToElement(item, document);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, documentPrototype, document);
        }

        private void ParseSentence(Document document, ConfigurationPair item)
        {
            var newSentence = ObjectCopier.Clone(sentencePrototype);

            AddAttributesToElement(item, newSentence);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, sentencePrototype, newSentence);

            document.Sentences.Add(newSentence);
        }

        private void ParseWord(Document document, ConfigurationPair item)
        {
            var newWord = ObjectCopier.Clone(wordPrototype);

            AddAttributesToElement(item, newWord);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, wordPrototype, newWord);

            var lastSentence = document.Sentences.Last();
            lastSentence.Words.Add(newWord);
        }

        private void AddAttributesToElement(ConfigurationPair item, Element elementToModify)
        {
            foreach (var itemAttributes in item.Attributes)
            {
                foreach (var itemAttribute in itemAttributes)
                {
                    var attrib0 = elementToModify.Attributes.FirstOrDefault(a => a.Name == itemAttribute.Key);

                    if (attrib0 != null)
                    {
                        attrib0.Value = itemAttribute.Value;
                    }
                    else
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

            var exceptionsFound = false;

            foreach (var wordPrototypeAttribute in elementPrototype.Attributes)
            {
                if (!wordPrototypeAttribute.IsOptional)
                {
                    var wordAttribute =
                        newElement.Attributes.SingleOrDefault(atr => atr.Name.Equals(wordPrototypeAttribute.Name));
                    if ((wordAttribute == null) || string.IsNullOrEmpty(wordAttribute.Value))
                    {
                        if ((document == null) || !document.Sentences.Any())
                        {
                            continue;
                        }

                        var lastSentence = document.Sentences.Last();

                        var newWordId = newElement.GetAttributeByName("id");
                        var sentenceId = lastSentence.GetAttributeByName("id");
                        var documentId = document.GetAttributeByName("id");

                        EventAggregator.GetEvent<ValidationExceptionEvent>()
                            .Publish(
                                string.Format(
                                    MissingNonOptionalAttributeErrorMessage,
                                    wordPrototypeAttribute.Name,
                                    newWordId,
                                    sentenceId,
                                    documentId));
                        exceptionsFound = true;
                    }
                }
            }

            if (exceptionsFound)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish("Please check warnings in the Output panel.");
            }
        }
    }
}