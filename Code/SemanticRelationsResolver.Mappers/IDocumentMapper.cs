namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;
    using Domain;

    public interface IDocumentMapper
    {
        Task<Document> Map(string filepath);
    }
}