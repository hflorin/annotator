namespace Treebank.Mappers.Configuration
{
    using System.Collections.Generic;

    public interface IAppConfig
    {
        string Name { get; set; }

        string Filepath { get; set; }

        IList<DataStructure> DataStructures { get; set; }

        IList<Definition> Definitions { get; set; }
    }
}