namespace Treebank.Mappers.Configuration
{
    using System.Collections.Generic;
    using Domain;

    public class DataStructure
    {
        public DataStructure()
        {
            Elements = new List<Element>();
        }

        public string Format { get; set; }
        public IList<Element> Elements { get; set; }
    }
}