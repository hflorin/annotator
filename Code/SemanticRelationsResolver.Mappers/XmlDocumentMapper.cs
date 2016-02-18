namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;

    using SemanticRelationsResolver.Domain;
    using SemanticRelationsResolver.Loaders;

    public class XmlDocumentMapper : IDocumentMapper
    {
        public IResourceLoader ResourceLoader { get; set; }

        public async Task<Document> Map(string filepath)
        {
            var documentContent = await ResourceLoader.LoadAsync(filepath);

            var document = new Document();

            return document;
        }
    }
}