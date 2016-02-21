namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;
    using Domain;

    public interface IDocumentMapper
    {
        Task<Treebank> Map(string filepath);
    }
}