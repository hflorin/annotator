namespace SemanticRelationsResolver.Domain.Configuration
{
    using System.Collections.Generic;

    public class AppConfig : IAppConfig
    {
        public AppConfig()
        {
            Elements = new List<Element>();
        }

        public IList<Element> Elements { get; set; }
    }
}