using System;

namespace SemanticRelationsResolver.Console.Config
{
    public class ConsoleConfig
    {
        private readonly IAppConfig _appConfig;

        public ConsoleConfig(IAppConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config", "Failed to initialize console application configuration.");
            }

            _appConfig = config;
        }

        public string InputDirectoryPath
        {
            get
            {
                return _appConfig.GetString("inputDirectoryPath");
            }
        }

        public string OutputDirectoryPath
        {
            get
            {
                return _appConfig.GetString("outputDirectoryPath");
            }
        }

    }
}
