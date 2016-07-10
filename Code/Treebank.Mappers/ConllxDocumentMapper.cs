namespace Treebank.Mappers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Domain;
    using Events;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class ConllxDocumentMapper : IDocumentMapper
    {
        private Document documentPrototype;
        private int sentenceIdProvider;

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
                appConfig.DataStructures.FirstOrDefault(d => d.Format == ConfigurationStaticData.ConllxFormat);

            if (datastructure == null)
            {
                EventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Could not load CONLLX file because the structure is not defined in the configuration file.");
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
                            document.Sentences.Add(ObjectCopier.Clone(sentencePrototype));
                            break;
                        case false :
                            AddWordToSentence(document, line);
                            break;
                    }
                }
            }
        }

        private void AddWordToSentence(Document document, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                document.Sentences.Remove(document.Sentences.LastOrDefault());
                return;
            }

            var lastSentence = document.Sentences.LastOrDefault();
            if (lastSentence != null)
            {
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

                lastSentence.Words.Add(newWord);

                if (string.IsNullOrWhiteSpace(lastSentence.GetAttributeByName("id")))
                {
                    lastSentence.SetAttributeByName("id", sentenceIdProvider++.ToString());
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
    }
}