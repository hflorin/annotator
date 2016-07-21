namespace Treebank.Persistence
{
    using System.Threading.Tasks;

    using Treebank.Domain;

    public interface IPersister
    {
        Task Save(Document document, string filepathToSaveTo = "", bool overwrite = true);
    }
}