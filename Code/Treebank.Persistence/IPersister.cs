namespace Treebank.Persistence
{
    using Domain;

    public interface IPersister
    {
        void Save(Document document, string filepath);
    }
}