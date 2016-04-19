namespace SemanticRelationsResolver.Mappers
{
    using System.Threading.Tasks;
    using Domain.Configuration;
    using Loaders;

    public class AppConfigMapper : IAppConfigMapper
    {
        public DynamicXmlLoader Loader { get; set; }

        public async Task<IAppConfig> Map(string filepath)
        {
            var appConfigContent = await Loader.LoadAsync(filepath);

            return await Task.Run(CreateAppConfig(appConfigContent));
        }

        private IAppConfig CreateAppConfig(dynamic appConfigContent)
        {
            return new AppConfig();
        }
    }
}