namespace Treebank.Persistence
{
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using Domain;

    public class XmlPersister : IPersister
    {
        public void Save(Document document, string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return;
            }

            using (var xmlWriter = new XmlTextWriter(filepath, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;

                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement(document.Name);
                WriteAttributes(document.Attributes, xmlWriter);

                foreach (var sentence in document.Sentences)
                {
                    xmlWriter.WriteStartElement(sentence.Name);
                    WriteAttributes(sentence.Attributes, xmlWriter);

                    foreach (var word in sentence.Words)
                    {
                        xmlWriter.WriteStartElement(word.Name);

                        WriteAttributes(word.Attributes, xmlWriter);

                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.Flush();
                xmlWriter.WriteEndDocument();
            }
        }

        private void WriteAttributes(ICollection<Attribute> attributes, XmlWriter writer)
        {
            var internalAttributes = new List<string> {"configuration", "content", "configurationFilePath" };

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