namespace Treebank.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    using Prism.Events;

    using Treebank.Domain;
    using Treebank.Mappers;
    using Treebank.Mappers.LightWeight;

    using Attribute = Treebank.Domain.Attribute;

    public class XmlPersister : IPersister
    {
        private IEventAggregator eventAggregator;

        public XmlPersister(IEventAggregator eventAggregator)
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

            var appConfigMapper = new AppConfigMapper();

            var configFilePath = document.GetAttributeByName("configurationFilePath");

            var documentMapper =
                new DocumentMapperClient(
                    new LightDocumentMapperWithReader
                        {
                            AppConfigMapper = appConfigMapper, 
                            EventAggregator = eventAggregator
                        });

            using (var xmlWriter = new XmlTextWriter(newFilepath, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;

                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement(document.Name);
                WriteAttributes(document.Attributes, xmlWriter);

                foreach (var sentence in document.Sentences)
                {
                    xmlWriter.WriteStartElement(sentence.Name);
                    WriteAttributes(sentence.Attributes, xmlWriter);

                    if (sentence.Words.Any())
                    {
                        WriteSentenceWord(sentence, xmlWriter);
                    }
                    else
                    {
                        var oldSentence =
                            await
                            documentMapper.LoadSentence(sentence.GetAttributeByName("id"), filepath, configFilePath);
                        WriteSentenceWord(oldSentence, xmlWriter);
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.Flush();
                xmlWriter.WriteEndDocument();
            }

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            File.Move(newFilepath, filepath);
        }

        private void WriteSentenceWord(Sentence sentence, XmlTextWriter xmlWriter)
        {
            foreach (var word in sentence.Words)
            {
                xmlWriter.WriteStartElement(word.Name);

                WriteAttributes(word.Attributes, xmlWriter);

                xmlWriter.WriteEndElement();
            }
        }

        private void WriteAttributes(ICollection<Attribute> attributes, XmlWriter writer)
        {
            var internalAttributes = new List<string> { "configuration", "content", "configurationFilePath" };

            foreach (var attribute in attributes)
            {
                if (internalAttributes.Contains(attribute.Name))
                {
                    continue;
                }

                writer.WriteAttributeString(attribute.Name, attribute.Value);
            }
        }
    }
}