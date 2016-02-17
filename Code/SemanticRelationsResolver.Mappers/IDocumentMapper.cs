namespace SemanticRelationsResolver.Mappers
{
    using Domain;

    public interface IDocumentMapper
    {
        Document Map(string filepath);
    }
}
