﻿namespace Treebank.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Configuration;
    using Domain;
    using Events;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class DocumentMapperWithReader : IDocumentMapper
    {
        private Document documentPrototype;

        private Sentence sentencePrototype;

        private Word wordPrototype;

        public IEventAggregator EventAggregator { get; set; }

        public IAppConfigMapper AppConfigMapper { get; set; }

        public static Dictionary<string, string> GetConfigFileNameToFilePathMapping()
        {
            var configFilesDirectoryPath = ConfigurationManager.AppSettings["configurationFilesDirectoryPath"];

            var filenameToPathMapping = new Dictionary<string, string>();

            if ((configFilesDirectoryPath != null) && Directory.Exists(configFilesDirectoryPath))
            {
                var configFilesPaths = Directory.GetFiles(configFilesDirectoryPath).ToList();

                foreach (var configFilePath in configFilesPaths)
                {
                    var filename = Path.GetFileName(configFilePath);
                    filenameToPathMapping.Add(configFilePath, filename);
                }
            }

            return filenameToPathMapping;
        }
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

            var document = await CreateDocument(filepath, appConfig);

            document.FilePath = filepath;

            var filenameToPathMapping = GetConfigFileNameToFilePathMapping();

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

            return document;
        }

        private static string GetEntityNameByElementName(IAppConfig appConfig, ConfigurationPair item)
        {
            return appConfig.Elements.Single(e => e.Name.Equals(item.ElementName)).Entity;
        }

        private async Task<Document> CreateDocument(string filepath, IAppConfig appConfig)
        {
            var document = ObjectCopier.Clone(documentPrototype);

            await ParseDocument(filepath, appConfig, document);

            await Task.Run(() => AddInternalAttributes(document));

            return document;
        }

        private async Task ParseDocument(string filepath, IAppConfig appConfig, Document document)
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
                            AddElementsToDocument(document, queue, appConfig);
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
                var entityName = GetEntityNameByElementName(appConfig, item);

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
            document.Attributes.Clear();

            AddAttributesToElement(item, documentPrototype, document);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, documentPrototype, document);
        }

        private void ParseSentence(Document document, ConfigurationPair item)
        {
            var newSentence = ObjectCopier.Clone(sentencePrototype);
            newSentence.Attributes.Clear();

            AddAttributesToElement(item, sentencePrototype, newSentence);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, sentencePrototype, newSentence);

            document.Sentences.Add(newSentence);
        }

        private void ParseWord(Document document, ConfigurationPair item)
        {
            var newWord = ObjectCopier.Clone(wordPrototype);
            newWord.Attributes.Clear();

            AddAttributesToElement(item, wordPrototype, newWord);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, wordPrototype, newWord);

            var lastSentence = document.Sentences.Last();
            lastSentence.Words.Add(newWord);
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