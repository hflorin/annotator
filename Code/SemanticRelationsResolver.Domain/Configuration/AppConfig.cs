namespace SemanticRelationsResolver.Domain.Configuration
{
    using System.Collections.Generic;

    public class AppConfig : IAppConfig
    {
        public AppConfig()
        {
            Elements = new List<Element>();
            Definitions = new List<Definition>();
        }

        public IList<Element> Elements { get; set; }

        public IList<Definition> Definitions { get; set; }
    }
}