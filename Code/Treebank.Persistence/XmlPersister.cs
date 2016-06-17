namespace Treebank.Persistence
{
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    using Treebank.Domain;

    public class XmlPersister : IPersister
    {
        public void Save(Document document, string filepath)
        {
            using (var xmlWriter = new XmlTextWriter(filepath, Encoding.UTF8))
            {
                xmlWriter.WriteStartElement(document.Name);

                WriteAttributes(document.Attributes, xmlWriter);

                foreach (var sentence in document.Sentences)
                {
                    xmlWriter.WriteStartElement(sentence.Name);

                    WriteAttributes(sentence.Attributes, xmlWriter);

                    foreach (var word in sentence.Words)
                    {
                        xmlWriter.WriteStartElement(sentence.Name);

                        WriteAttributes(word.Attributes, xmlWriter);

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
            }
        }

        private void WriteAttributes(ICollection<Attribute> attributes, XmlTextWriter writer)
        {
            foreach (var attribute in attributes)
            {
                writer.WriteAttributeString(attribute.Name, attribute.Value);
            }
        }
    }
}