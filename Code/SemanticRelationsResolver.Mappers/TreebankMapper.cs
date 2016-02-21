namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;
    using Domain;
    using Loaders;

    public class TreebankMapper : IDocumentMapper
    {
        public IResourceLoader ResourceLoader { get; set; }

        public async Task<Treebank> Map(string filepath)
        {
            var documentContent = await ResourceLoader.LoadAsync(filepath);

            return new Treebank(documentContent);
        }
    }
}