namespace Treebank.Mappers.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Configuration;
    using Domain;
    using Events;
    using Models;
    using Prism.Events;
    using Attribute = Domain.Attribute;
    using Sentence = Domain.Sentence;
    using Word = Domain.Word;

    public class SerializationDocumentMapper : IDocumentMapper
    {
        private Definition definition;

        private Document documentPrototype;

        private Sentence sentencePrototype;

        private Word wordPrototype;

        public IEventAggregator EventAggregator { get; set; }

        public IAppConfigMapper AppConfigMapper { get; set; }

        public async Task<Document> Map(string filepath, string configFilepath, DataStructure dataStructure = null,
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
                                    d => d.Format.Equals(ConfigurationStaticData.XmlFormat, StringComparison.InvariantCultureIgnoreCase));

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
                new Domain.Attribute
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
                new Domain.Attribute
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

        private Treebank LoadTreebank(string filepath)
        {
            var ser = new XmlSerializer(typeof(Treebank));
            using (var reader = XmlReader.Create(filepath))
            {
                return (Treebank)ser.Deserialize(reader);
            }
        }

        private Task<Document> CreateDocument(string filepath, DataStructure datastructure)
        {
            var treebank = LoadTreebank(filepath);

            return Adaptor(treebank);
        }

        private async Task<Document> Adaptor(Treebank input)
        {
            return await Task.Run(() =>
            {
                var result = ObjectCopier.Clone(documentPrototype);
                result.SetAttributeByName("id", input.Id);

                foreach (var inputSentence in input.Sentences)
                {
                    var sentence = ObjectCopier.Clone(sentencePrototype);

                    sentence.SetAttributeByName("id", inputSentence.Id);
                    sentence.SetAttributeByName("citation-part", inputSentence.CitationPart);
                    sentence.SetAttributeByName("date", inputSentence.Date);
                    sentence.SetAttributeByName("parser", inputSentence.Parser);
                    sentence.SetAttributeByName("user", inputSentence.User);

                    AddSentenceInternalAttributes(sentence, inputSentence);

                    result.Sentences.Add(sentence);
                }

                return result;
            });
        }

        private void AddSentenceInternalAttributes(Sentence sentence, Models.Sentence input)
        {
            var sentenceContent = string.Empty;
            foreach (var word in input.Words)
            {
                sentenceContent += word.Form + " ";
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


        public async Task<Sentence> LoadSentence(string sentenceId, string filepath, string configFilepath,
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
                                    d => d.Format.Equals(ConfigurationStaticData.XmlFormat, StringComparison.InvariantCultureIgnoreCase));

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

            var treebank = LoadTreebank(filepath);

            var inputSentence = treebank.Sentences.FirstOrDefault(s => s.Id.Equals(sentenceId,StringComparison.InvariantCultureIgnoreCase));

            var sentence = ObjectCopier.Clone(sentencePrototype);

            if (inputSentence == null)
            {
                return sentence;
            }

            AddSentenceInternalAttributes(sentence, inputSentence);
                
            sentence.SetAttributeByName("id", inputSentence.Id);
            sentence.SetAttributeByName("citation-part", inputSentence.CitationPart);
            sentence.SetAttributeByName("date", inputSentence.Date);
            sentence.SetAttributeByName("parser", inputSentence.Parser);
            sentence.SetAttributeByName("user", inputSentence.User);

            foreach (var inputSentenceWord in inputSentence.Words)
            {
                var word = ObjectCopier.Clone(wordPrototype);

                word.SetAttributeByName("id", inputSentenceWord.Id);
                word.SetAttributeByName("chunk", inputSentenceWord.Chunk);
                word.SetAttributeByName("deprel", inputSentenceWord.DepRel);
                word.SetAttributeByName("form", inputSentenceWord.Form);
                word.SetAttributeByName("head", inputSentenceWord.Head);
                word.SetAttributeByName("lemma", inputSentenceWord.Lemma);
                word.SetAttributeByName("postag", inputSentenceWord.Postag);

                sentence.Words.Add(word);
            }

            return sentence;
        }
    }
}