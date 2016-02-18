namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;

    using SemanticRelationsResolver.Domain;

    public interface IDocumentMapper
    {
        Task<Document> Map(string filepath);
    }
}