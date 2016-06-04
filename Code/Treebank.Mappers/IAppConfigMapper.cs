namespace Treebank.Mappers
{
    using System.Threading.Tasks;
    using Configuration;

    public interface IAppConfigMapper
    {
        Task<IAppConfig> Map(string filepath);
    }
}