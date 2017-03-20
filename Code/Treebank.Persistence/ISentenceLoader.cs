using System.Threading.Tasks;
using Treebank.Domain;

namespace Treebank.Persistence
{
    public interface ISentenceLoader
    {
        Task<Sentence> LoadSentenceWords(string sentenceId, string documentFilePath, string configFilePath);
    }
}
