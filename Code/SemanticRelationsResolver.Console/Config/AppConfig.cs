namespace SemanticRelationsResolver.Console.Config
{
    using System.Collections.Specialized;
    using System.Configuration;

    public class AppConfig : IAppConfig
    {
        private static readonly NameValueCollection FilePaths =
            ConfigurationManager.GetSection("filePaths") as NameValueCollection;

        public string GetString(string key)
        {
            return FilePaths != null ? FilePaths[key] : string.Empty;
        }
    }
}