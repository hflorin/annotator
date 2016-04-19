namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;
    using Domain.Configuration;

    public interface IAppConfigMapper
    {
        Task<IAppConfig> Map(string filepath);
    }
}