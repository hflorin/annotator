namespace Treebank.Mappers
{
    using System.Threading.Tasks;
    using Configuration;
    using Domain;

    public interface IDocumentMapper
    {
        Task<Document> Map(string filepath, string configFilepath, DataStructure dataStructure = null, Definition definition = null);
        Task<Sentence> LoadSentence(string sentenceId, string filepath, string configFilepath, DataStructure dataStructure = null, Definition definition = null);
    }
}