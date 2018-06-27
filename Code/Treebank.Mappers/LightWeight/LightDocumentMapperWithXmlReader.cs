namespace Treebank.Mappers.LightWeight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Algos;
    using Configuration;
    using Domain;
    using Events;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class LightDocumentMapperWithXmlReader : IDocumentMapper
    {
        private Definition definition;

        private Document documentPrototype;

        private Sentence sentencePrototype;

        private Word wordPrototype;

        public IEventAggregator EventAggregator { get; set; }

        public IAppConfigMapper AppConfigMapper { get; set; }

        public async Task<Document> Map(
            string filepath,
            string configFilepath,
            DataStructure dataStructure = null,
            Definition definitionParam = null)
        {
            var appConfig = await AppConfigMapper.Map(configFilepath);

            if (appConfig == null)
            {
                throw new ArgumentNullException(
                    "configFilepath",
                    string.Format("Could not load configuration file from: {0}", configFilepath));
            }

            var datastructure = dataStructure
                                ?? appConfig.DataStructures.FirstOrDefault(
                                    d => d.Format.Equals(ConfigurationStaticData.XmlFormat,StringComparison.InvariantCultureIgnoreCase));

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish("Could not load XML file because the structure is not defined in the configuration file.");
                return null;
            }

            definition = definitionParam ?? appConfig.Definitions.FirstOrDefault();

            if (definition == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load XML file because the tree definitionParam is not defined in the configuration file.");
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

        public async Task<Sentence> LoadSentence(
            string sentenceId,
            string filepath,
            string configFilepath,
            DataStructure dataStructure = null,
            Definition definitionParam = null)
        {
            var appConfig = await AppConfigMapper.Map(configFilepath);

            if (appConfig == null)
            {
                throw new ArgumentNullException(
                    "configFilepath",
                    string.Format("Could not load configuration file from: {0}", configFilepath));
            }

            var datastructure =
                appConfig.DataStructures.FirstOrDefault(d => d.Format.Equals(ConfigurationStaticData.XmlFormat, StringComparison.InvariantCultureIgnoreCase));

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load CONLLX file because the structure is not defined in the configuration file.");
                return null;
            }

            definition = definitionParam ?? appConfig.Definitions.FirstOrDefault();

            if (definition == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load XML file because the tree definitionParam is not defined in the configuration file.");
                return null;
            }

            wordPrototype = datastructure.Elements.OfType<Word>().Single();
            sentencePrototype = datastructure.Elements.OfType<Sentence>().Single();

            var sentence = await Task.FromResult(CreateSentence(filepath, sentenceId, datastructure));

            return sentence;
        }

        private static string GetEntityNameByElementName(DataStructure dataStructure, ConfigurationPair item)
        {
            var element = dataStructure.Elements.FirstOrDefault(e => e.Name.Equals(item.ElementName));
            return element!=null? element.Entity:string.Empty;
        }

        private Sentence CreateSentence(string filepath, string sentenceId, DataStructure dataStructure)
        {
            using (var reader = new XmlTextReader(filepath))
            {
                var queue = new List<ConfigurationPair>();
                Sentence sentence = null;

                while (reader.Read())
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
                            sentence = CreateSentence(queue, dataStructure, sentenceId);
                            break;
                    }

                    if (sentence != null)
                    {
                        ProcessPreviousSentence(sentence);

                        return sentence;
                    }
                }
            }

            return null;
        }

        private Sentence CreateSentence(
            ICollection<ConfigurationPair> queue,
            DataStructure dataStructure,
            string sentenceId)
        {
            if (!queue.Any())
            {
                return null;
            }

            Sentence sentence = null;

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
                    if (sentence == null)
                    {
                        continue;
                    }

                    ParseWord(sentence, item);
                }
                else if (entity is Sentence)
                {
                    sentence = ParseSentence(item);

                    if ((sentence != null) && (sentence.GetAttributeByName("id") != sentenceId))
                    {
                        sentence = null;
                        break;
                    }
                }
            }

            queue.Clear();

            return sentence;
        }

        private async Task<Document> CreateDocument(string filepath, DataStructure dataStructure)
        {
            var document = ObjectCopier.Clone(documentPrototype);

            await ParseDocument(filepath, dataStructure, document);

            // await Task.Run(() => AddDocumentInternalAttributes(document));
            return document;
        }

        private async Task ParseDocument(string filepath, DataStructure dataStructure, Document document)
        {
            using (var reader = XmlReader.Create(filepath))
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

            ProcessPreviousSentence(document);
        }

        private void AddWordInternalAttributes(Word word)
        {
            var formAttribute = word.Attributes.SingleOrDefault(a => a.Name.Equals("form"));
            var formValue = formAttribute != null ? formAttribute.Value : string.Empty;
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

        private void AddSentenceInternalAttributes(Sentence sentence)
        {
            var sentenceContent = string.Empty;
            foreach (var word in sentence.Words)
            {
                AddWordInternalAttributes(word);
                sentenceContent += word.GetAttributeByName("content") + " ";
            }

            sentence.Attributes.Add(
                new Attribute
                {
                    Name = "content",
                    DisplayName = "Content",
                    Value = sentenceContent.TrimEnd(),
                    IsOptional = true,
                    IsEditable = false
                });
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
            ProcessPreviousSentence(document);

            var newSentence = ObjectCopier.Clone(sentencePrototype);

            AddAttributesToElement(item, newSentence);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, sentencePrototype, newSentence);

            document.Sentences.Add(newSentence);
        }

        private Sentence ParseSentence(ConfigurationPair item)
        {
            var newSentence = ObjectCopier.Clone(sentencePrototype);

            AddAttributesToElement(item, newSentence);

            return newSentence;
        }

        private void ProcessPreviousSentence(Document document)
        {
            var previousSentece = document.Sentences.LastOrDefault();

            if ((previousSentece == null) || (previousSentece.Words.Count <= 1))
            {
                return;
            }

            ProcessPreviousSentence(previousSentece);

            previousSentece.Words.Clear();
        }

        private void ProcessPreviousSentence(Sentence sentence)
        {
            if ((sentence == null) || (sentence.Words.Count <= 1))
            {
                return;
            }

            AddSentenceInternalAttributes(sentence);

            EventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(
                    string.Format(
                        "Loaded sentence: {0} {1}",
                        sentence.GetAttributeByName("id"),
                        sentence.GetAttributeByName("content")));

            var validationResult = new CheckGraphResult();

            sentence.IsTree = GraphOperations.GetGraph(sentence, definition, EventAggregator).IsTree(validationResult);

            ProcessGraphValidationResult(validationResult, sentence.GetAttributeByName("id"));
        }

        private void ProcessGraphValidationResult(CheckGraphResult validationResult, string sentenceId)
        {
            foreach (var disconnectedWordId in validationResult.DisconnectedWordIds)
            {
                EventAggregator.GetEvent<ValidationExceptionEvent>()
                    .Publish(
                        string.Format(
                            "The word id: {0}, in sentence id: {1}, is not connected to another word.",
                            disconnectedWordId,
                            sentenceId));
            }

            foreach (var cycle in validationResult.Cycles)
            {
                EventAggregator.GetEvent<ValidationExceptionEvent>()
                    .Publish(
                        string.Format("The sentence with id {0} has cycle: {1}", sentenceId, string.Join(",", cycle)));
            }
        }

        private void ParseWord(Document document, ConfigurationPair item)
        {
            var newWord = ObjectCopier.Clone(wordPrototype);

            AddAttributesToElement(item, newWord);
            NotifyIfAnyNonOptionalAttributeIsMissing(document, wordPrototype, newWord);

            var lastSentence = document.Sentences.Last();
            lastSentence.Words.Add(newWord);
        }

        private void ParseWord(Sentence sentence, ConfigurationPair item)
        {
            var newWord = ObjectCopier.Clone(wordPrototype);

            AddAttributesToElement(item, newWord);

            sentence.Words.Add(newWord);
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

            foreach (var attribute in elementToModify.Attributes)
            {
                if (!attribute.ExceptedValuesOfSet.Any())
                {
                    continue;
                }

                foreach (var exceptValuesOf in attribute.ExceptedValuesOfSet)
                {
                    var attributeDependency = elementToModify.Attributes.FirstOrDefault(a => a.Name.Equals(exceptValuesOf.AttributeName, StringComparison.InvariantCulture));

                    if (attributeDependency == null)
                    {
                        return;
                    }

                    if (exceptValuesOf.Values.Contains(attributeDependency.Value))
                    {
                        attribute.IsEditable = false;
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
                    }
                }
            }
        }
    }
}