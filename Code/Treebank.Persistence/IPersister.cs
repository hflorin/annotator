namespace Treebank.Persistence
{
    using Treebank.Domain;

    public interface IPersister
    {
        void Save(Document document, string filepath);
    }
}