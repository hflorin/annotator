namespace SemanticRelationsResolver.Loaders
{
    public interface IResourceLoader
    {
        dynamic Load(string path);
    }
}