namespace SemanticRelationsResolver.Domain.Configuration
{
    using System.Collections.Generic;

    public interface IAppConfig
    {
        IList<Element> Elements { get; set; }

        IList<Definition> Definitions { get; set; }
    }
}