namespace Treebank.Mappers
{
    using System.Threading.Tasks;
    using Treebank.Domain;

    public interface IDocumentMapper
    {
        Task<Document> Map(string filepath, string configFilepath);
    }
}