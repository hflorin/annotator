namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;
    using Domain;
    using Loaders;

    public class TreebankMapper : IDocumentMapper
    {
        public IResourceLoader ResourceLoader { get; set; }

        public async Task<Document> Map(string filepath)
        {
            var documentContent = await ResourceLoader.LoadAsync(filepath);

            return new Document(documentContent);
        }
    }
}