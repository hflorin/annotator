 namespace Treebank.Mappers.Configuration
{
    using System.Collections.Generic;
    using Domain;

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