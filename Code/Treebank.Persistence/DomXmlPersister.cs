namespace Treebank.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Domain;

    public class DomXmlPersister : IPersister
    {
        private static readonly List<string> AttributesThatMustNotBeSaved = new List<string>
        {
            "configuration",
            "content",
            "configurationFilePath"
        };

        public async Task Save(Document document, string filepathToSaveTo = "", bool overwrite = true)
        {
            try
            {
                var filepath = string.IsNullOrWhiteSpace(filepathToSaveTo) ? document.FilePath : filepathToSaveTo;

                if (string.IsNullOrWhiteSpace(filepath))
                {
                    return;
                }

                var fileName = Path.GetFileName(filepath);
                var newFilepath = filepath.Replace(fileName, "New" + fileName);

                var loadXmlDocumentTask = Task.Run(() => LoadXmlDocument(document));
                var doc = await loadXmlDocumentTask;

                var processSentencesTask = Task.Run(() => ProcessSentences(document, doc));
                await processSentencesTask;

                var persistDocumentToFiletask = Task.Run(() => PersistDocumentToFile(doc, newFilepath, filepath));
                await persistDocumentToFiletask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<XmlDocument> LoadXmlDocument(Document document)
        {
            var doc = new XmlDocument();

            await Task.Run(() => doc.Load(document.FilePath));

            return doc;
        }

        private static async Task ProcessSentences(Document document, XmlDocument doc)
        {
            foreach (var sentence in document.Sentences)
            {
                if (IsNewSentence(sentence, doc))
                {
                    AddNewSentenceToDocument(sentence, doc);
                }
                else if (IsModifiedSentence(sentence))
                {
                    SaveModifiedSentenceToDocument(sentence, doc);
                }
            }

            var deleteRemovedSentencesTask =
                Task.Run(() => DeleteSentencesFromDocumentIfRemovedInCurrentSession(doc, document));
            await deleteRemovedSentencesTask;
        }

        //todo: very no bueno using hardcoded entity element names, like sentence, word, need to use appconfig 
        private static void AddNewSentenceToDocument(Sentence sentence, XmlDocument doc)
        {
            var sentenceXmlNode = doc.CreateElement("sentence");

            foreach (var attribute in sentence.Attributes)
            {
                if (AttributesThatMustNotBeSaved.Contains(attribute.Name))
                {
                    continue;
                }

                var newAttr = sentenceXmlNode.OwnerDocument?.CreateAttribute(attribute.Name);

                if (newAttr == null)
                {
                    continue;
                }

                newAttr.Value = attribute.Value;
                sentenceXmlNode.Attributes.SetNamedItem(newAttr);
            }

            foreach (var word in sentence.Words)
            {
                var wordXmlElement = doc.CreateElement("word");
                foreach (var attribute in word.Attributes)
                {
                    if (AttributesThatMustNotBeSaved.Contains(attribute.Name))
                    {
                        continue;
                    }

                    wordXmlElement.SetAttribute(attribute.Name, attribute.Value);
                }

                sentenceXmlNode.AppendChild(wordXmlElement);
            }

            doc.DocumentElement?.AppendChild(sentenceXmlNode);
        }

        private static bool IsNewSentence(Element sentence, XmlNode doc)
        {
            return doc.SelectSingleNode($"/treebank/sentence[@id='{sentence.GetAttributeByName("id")}']") == null;
        }

        private static bool IsModifiedSentence(Sentence sentence)
        {
            return sentence.Words.Any();
        }

        private static void SaveModifiedSentenceToDocument(Sentence sentence, XmlDocument doc)
        {
            var sentenceXmlNode =
                doc.SelectSingleNode($"/treebank/sentence[@id='{sentence.GetAttributeByName("id")}']");

            if (sentenceXmlNode == null)
            {
                return;
            }

            sentenceXmlNode.RemoveAll();

            foreach (var attribute in sentence.Attributes)
            {
                if (sentenceXmlNode.OwnerDocument == null)
                {
                    continue;
                }

                if (AttributesThatMustNotBeSaved.Contains(attribute.Name))
                {
                    continue;
                }

                var newAttr = sentenceXmlNode.OwnerDocument.CreateAttribute(attribute.Name);
                newAttr.Value = attribute.Value;
                sentenceXmlNode.Attributes?.SetNamedItem(newAttr);
            }

            foreach (var word in sentence.Words)
            {
                var wordXmlElement = doc.CreateElement("word");
                foreach (var attribute in word.Attributes)
                {
                    if (AttributesThatMustNotBeSaved.Contains(attribute.Name))
                    {
                        continue;
                    }

                    wordXmlElement.SetAttribute(attribute.Name, attribute.Value);
                }

                sentenceXmlNode.AppendChild(wordXmlElement);
            }
        }

        private static void DeleteSentencesFromDocumentIfRemovedInCurrentSession(XmlNode doc, Document document)
        {
            var persistedSentencesIds = doc.SelectNodes("/treebank/sentence[@id]/@id");
            if (persistedSentencesIds == null)
            {
                return;
            }

            foreach (XmlNode persistedSentenceId in persistedSentencesIds)
            {
                if (document.Sentences.Any(s => s.GetAttributeByName("id").Equals(persistedSentenceId.Value)))
                {
                    continue;
                }

                var nodeToRemove = doc.SelectSingleNode($"/treebank/sentence[@id='{persistedSentenceId.Value}']");

                if (nodeToRemove != null)
                {
                    doc.RemoveChild(nodeToRemove);
                }
            }
        }

        private static void PersistDocumentToFile(XmlDocument doc, string newFilepath, string filepath)
        {
            doc.Save(newFilepath);

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            File.Move(newFilepath, filepath);
        }
    }
}