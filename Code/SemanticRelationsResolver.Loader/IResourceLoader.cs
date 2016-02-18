namespace SemanticRelationsResolver.Loaders
{
    using System.Threading.Tasks;

    public interface IResourceLoader
    {
        Task<dynamic> LoadAsync(string path);
    }
}