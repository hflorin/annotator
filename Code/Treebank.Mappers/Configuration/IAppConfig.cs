namespace Treebank.Mappers.Configuration
{
    using System.Collections.Generic;
    using Domain;

    public interface IAppConfig
    {
        string Name { get; set; }

        string Filepath { get; set; }

        IList<Element> Elements { get; set; }

        IList<Definition> Definitions { get; set; }
    }
}