using System.Collections.Specialized;
using System.Configuration;

namespace SemanticRelationsResolver.Console.Config
{
    public class AppConfig : IAppConfig
    {
        private static readonly NameValueCollection FilePaths = ConfigurationManager.GetSection("filePaths") as NameValueCollection;

        public string GetString(string key)
        {
            return FilePaths != null ? FilePaths[key] : string.Empty;
        }
    }
}
