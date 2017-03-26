namespace Treebank.Persistence
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Domain;
    using Mappers;
    using Mappers.LightWeight;
    using Prism.Events;

    public class DomXmlPersister : IPersister
    {
        private readonly IEventAggregator eventAggregator;

        public DomXmlPersister(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");

            this.eventAggregator = eventAggregator;
        }

        public async Task Save(Document document, string filepathToSaveTo = "", bool overwrite = true)
        {
            var filepath = string.IsNullOrWhiteSpace(filepathToSaveTo) ? document.FilePath : filepathToSaveTo;

            if (string.IsNullOrWhiteSpace(filepath))
                return;

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

            var doc = new XmlDocument();
            doc.Load(document.FilePath);

            foreach (var sentence in document.Sentences)
            {
                if (!sentence.Words.Any()) continue;

                var sentenceXmlNode =
                    doc.SelectSingleNode($"/treebank/sentence[@id='{sentence.GetAttributeByName("id")}']");

                if (sentenceXmlNode == null) continue;

                sentenceXmlNode.RemoveAll();

                foreach (var attribute in sentence.Attributes)
                {
                    if (sentenceXmlNode.OwnerDocument == null) continue;

                    var newAttr = sentenceXmlNode.OwnerDocument.CreateAttribute(attribute.Name);
                    newAttr.Value = attribute.Value;
                    sentenceXmlNode.Attributes?.SetNamedItem(newAttr);
                }

                foreach (var word in sentence.Words)
                {
                    var wordXmlElement = doc.CreateElement("word");
                    foreach (var attribute in word.Attributes)
                    {
                        wordXmlElement.SetAttribute(attribute.Name, attribute.Value);
                    }
                    sentenceXmlNode.AppendChild(wordXmlElement);
                }
            }

            doc.Save(newFilepath);

            if (File.Exists(filepath))
                File.Delete(filepath);

            File.Move(newFilepath, filepath);
        }
    }
}