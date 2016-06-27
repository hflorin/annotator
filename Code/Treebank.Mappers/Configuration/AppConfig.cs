namespace Treebank.Mappers.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Domain;

    public class AppConfig : IAppConfig
    {
        public AppConfig()
        {
            Elements = new List<Element>();
            Definitions = new List<Definition>();
        }

        public string Name { get; set; }

        public string Filepath { get; set; }

        public IList<Element> Elements { get; set; }

        public IList<Definition> Definitions { get; set; }

        public static Dictionary<string, string> GetConfigFileNameToFilePathMapping()
        {
            var configFilesDirectoryPath = ConfigurationManager.AppSettings["configurationFilesDirectoryPath"];

            var filenameToPathMapping = new Dictionary<string, string>();

            if ((configFilesDirectoryPath != null) && Directory.Exists(configFilesDirectoryPath))
            {
                var configFilesPaths = Directory.GetFiles(configFilesDirectoryPath).ToList();

                foreach (var configFilePath in configFilesPaths)
                {
                    var filename = Path.GetFileName(configFilePath);
                    filenameToPathMapping.Add(configFilePath, filename);
                }
            }

            return filenameToPathMapping;
        }
    }
}