namespace Treebank.Mappers.Configuration
{
    using System.Collections.Generic;
    using Domain;

    public interface IAppConfig
    {
        IList<Element> Elements { get; set; }

        IList<Definition> Definitions { get; set; }
    }
}