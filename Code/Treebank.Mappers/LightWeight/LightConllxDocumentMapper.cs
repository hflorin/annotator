namespace Treebank.Mappers.LightWeight
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Algos;
    using Configuration;
    using Domain;
    using Events;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class LightConllxDocumentMapper : IDocumentMapper
    {
        private Definition definition;
        private Document documentPrototype;
        private int sentenceIdProvider;

        private Sentence sentencePrototype;

        private Word wordPrototype;

        public IEventAggregator EventAggregator { get; set; }

        public IAppConfigMapper AppConfigMapper { get; set; }

        public async Task<Document> Map(string filepath, string configFilepath, DataStructure dataStructure = null,
            Definition definition = null)
        {
            var appConfig = await AppConfigMapper.Map(configFilepath);

            if (appConfig == null)
            {
                throw new ArgumentNullException(
                    "configFilepath",
                    string.Format("Could not load configuration file from: {0}", configFilepath));
            }

            var datastructure =
                appConfig.DataStructures.FirstOrDefault(
                    d =>
                        (d.Format == ConfigurationStaticData.ConllxFormat) ||
                        (d.Format == ConfigurationStaticData.ConllFormat));

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load CONLLX file because the structure is not defined in the configuration file.");
                return null;
            }
            this.definition = definition ?? appConfig.Definitions.FirstOrDefault();

            if (this.definition == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load XML file because the tree definition is not defined in the configuration file.");
                return null;
            }


            wordPrototype = datastructure.Elements.OfType<Word>().Single();
            sentencePrototype = datastructure.Elements.OfType<Sentence>().Single();
            documentPrototype = datastructure.Elements.OfType<Document>().Single();

            var document = await CreateDocument(filepath);

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

        public async Task<Sentence> LoadSentence(string sentenceId, string filepath, string configFilepath,
            DataStructure dataStructure = null,
            Definition definition = null)
        {
            var appConfig = await AppConfigMapper.Map(configFilepath);

            if (appConfig == null)
            {
                throw new ArgumentNullException(
                    "configFilepath",
                    string.Format("Could not load configuration file from: {0}", configFilepath));
            }

            var datastructure =
                appConfig.DataStructures.FirstOrDefault(
                    d =>
                        (d.Format == ConfigurationStaticData.ConllxFormat) ||
                        (d.Format == ConfigurationStaticData.ConllFormat));

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load CONLLX file because the structure is not defined in the configuration file.");
                return null;
            }
            this.definition = definition ?? appConfig.Definitions.FirstOrDefault();

            if (this.definition == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load XML file because the tree definition is not defined in the configuration file.");
                return null;
            }


            wordPrototype = datastructure.Elements.OfType<Word>().Single();
            sentencePrototype = datastructure.Elements.OfType<Sentence>().Single();

            var sentence = await Task.FromResult(CreateSentence(filepath, sentenceId));

            return sentence;
        }

        private Sentence CreateSentence(string filepath, string sentenceId)
        {
            using (var reader = new StreamReader(filepath))
            {
                var sentence = ObjectCopier.Clone(sentencePrototype);

                sentence.SetAttributeByName("id", sentenceId);

                var currentSentenceId = 0;
                int sentenceIdNumber;

                if (int.TryParse(sentenceId, out sentenceIdNumber))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            currentSentenceId++;
                            continue;
                        }

                        if (currentSentenceId > sentenceIdNumber)
                        {
                            break;
                        }

                        if (currentSentenceId == sentenceIdNumber)
                        {
                            AddWordsToSentence(sentence, line);
                        }
                    }
                }
                return sentence;
            }
        }

        private async Task<Document> CreateDocument(string filepath)
        {
            var document = ObjectCopier.Clone(documentPrototype);

            if (string.IsNullOrWhiteSpace(document.GetAttributeByName("id")))
            {
                document.SetAttributeByName("id", Path.GetFileName(filepath));
            }

            await ParseDocument(filepath, document);

            await Task.Run(() => AddInternalAttributes(document));

            return document;
        }

        private async Task ParseDocument(string filepath, Document document)
        {
            using (var reader = new StreamReader(filepath))
            {
                string line;
                document.Sentences.Add(ObjectCopier.Clone(sentencePrototype));
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    switch (string.IsNullOrWhiteSpace(line))
                    {
                        case true :
                            ProcessPreviousSentence(document);
                            document.Sentences.Add(ObjectCopier.Clone(sentencePrototype));
                            break;
                        case false :
                            AddSentenceToDocument(document, line);
                            break;
                    }
                }
            }
        }

        private void ProcessPreviousSentence(Document document)
        {
            var previousSentece = document.Sentences.LastOrDefault();

            if ((previousSentece == null) || (previousSentece.Words.Count <= 1))
            {
                return;
            }

            AddSentenceInternalAttributes(previousSentece);

            EventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(string.Format("Loaded sentence: {0} {1}", previousSentece.GetAttributeByName("id"),
                    previousSentece.GetAttributeByName("content")));

            var validationResult = new CheckGraphResult();

            previousSentece.IsTree =
                GraphOperations.GetGraph(previousSentece, definition, EventAggregator).IsTree(validationResult);

            ProcessGraphValidationResult(validationResult, previousSentece.GetAttributeByName("id"));

            previousSentece.Words.Clear();
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
                        string.Format(
                            "The sentence with id {0} has cycle: {1}",
                            sentenceId,
                            string.Join(",", cycle)));
            }

            if (validationResult.DisconnectedWordIds.Any() || validationResult.Cycles.Any())
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish("Please check warnings in the Output panel.");
            }
        }

        private void AddSentenceToDocument(Document document, string line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
            {
                return;
            }

            var lastSentence = document.Sentences.LastOrDefault();
            if (lastSentence != null)
            {
                AddWordsToSentence(lastSentence, line);
            }
        }

        private void AddWordsToSentence(Sentence sentence, string line)
        {
            if (sentence == null)
            {
                return;
            }

            var tokens = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

            var newWord = ObjectCopier.Clone(wordPrototype);

            for (var i = 0; i < tokens.Length; i++)
            {
                var attribute = newWord.Attributes.FirstOrDefault(a => a.Position == i);
                if (attribute != null)
                {
                    attribute.Value = tokens[i];
                }
            }

            sentence.Words.Add(newWord);

            if (string.IsNullOrWhiteSpace(sentence.GetAttributeByName("id")))
            {
                sentence.SetAttributeByName("id", sentenceIdProvider++.ToString());
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
    }
}