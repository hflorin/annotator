namespace SemanticRelationsResolver.Mappers
{
    using Domain;
    using Loaders;

    public class XmlDocumentMapper : IDocumentMapper
    {
        public IResourceLoader ResourceLoader { get; set; }

        public Document Map(string filepath)
        {
            var documentContent = ResourceLoader.Load(filepath);

            var document = new Document();

            return document;
        }
    }
}