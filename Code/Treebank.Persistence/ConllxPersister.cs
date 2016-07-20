namespace Treebank.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Prism.Events;

    using Treebank.Domain;
    using Treebank.Mappers;
    using Treebank.Mappers.LightWeight;

    public class ConllxPersister : IPersister
    {
        private IEventAggregator eventAggregator;

        private Word wordPrototype;

        public ConllxPersister(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this.eventAggregator = eventAggregator;
        }

        public async Task Save(Document document, string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return;
            }

            var fileName = Path.GetFileName(filepath);
            var newFilepath = filepath.Replace(fileName, "New" + fileName);

            var mapper = new AppConfigMapper();

            var configFilePath = document.GetAttributeByName("configurationFilePath");

            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                return;
            }

            var appConfig = mapper.Map(configFilePath).GetAwaiter().GetResult();
            var dataStructure =
                appConfig.DataStructures.FirstOrDefault(
                    d =>
                    d.Format.Equals(ConfigurationStaticData.ConllxFormat)
                    || d.Format.Equals(ConfigurationStaticData.ConllFormat));

            if (dataStructure == null)
            {
                return;
            }

            wordPrototype = dataStructure.Elements.OfType<Word>().Single();

            var documentMapper =
                new DocumentMapperClient(
                    new LightConllxDocumentMapper { AppConfigMapper = mapper, EventAggregator = eventAggregator });

            using (var writer = new StreamWriter(newFilepath))
            {
                foreach (var sentence in document.Sentences)
                {
                    if (!sentence.Words.Any())
                    {
                        var oldSentence =
                            await
                            documentMapper.LoadSentence(sentence.GetAttributeByName("id"), filepath, configFilePath);

                        WriteSentenceWords(oldSentence, writer);
                    }
                    else
                    {
                        WriteSentenceWords(sentence, writer);
                    }
                }

                writer.Flush();
            }

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            File.Move(newFilepath, filepath);
        }

        private void WriteSentenceWords(Sentence sentence, StreamWriter writer)
        {
            if (sentence == null)
            {
                return;
            }

            foreach (var word in sentence.Words)
            {
                var wordLine = GetWordLine(word);
                writer.WriteLine(wordLine);
            }

            writer.WriteLine(string.Empty);
        }

        private string GetWordLine(Word word)
        {
            var result = new StringBuilder();

            var internalAttributes = new List<string> { "configuration", "content", "configurationFilePath" };

            var sortedAttributes = wordPrototype.Attributes.ToList();

            sortedAttributes.Sort((left, right) => left.Position.CompareTo(right.Position));

            foreach (var attribute in sortedAttributes)
            {
                if (internalAttributes.Contains(attribute.Name))
                {
                    continue;
                }

                var attributeFromWord = word.Attributes.FirstOrDefault(a => a.Name.Equals(attribute.Name));

                var attributeValue = attributeFromWord != null ? attributeFromWord.Value : string.Empty;

                if ((attributeFromWord != null) && !string.IsNullOrEmpty(attributeFromWord.Value))
                {
                    var splits = attributeFromWord.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splits.Length > 1)
                    {
                        attributeValue = string.Join("_", splits);
                    }
                }

                result.AppendFormat("{0}\t", attributeFromWord != null ? attributeValue : "_");
            }

            return result.ToString().TrimEnd('\t');
        }
    }
}