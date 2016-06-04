namespace Treebank.Mappers
{
    using System.Collections.Generic;

    public class ConfigurationPair
    {
        public ConfigurationPair()
        {
            Attributes = new List<Dictionary<string, string>>();
        }

        public string ElementName { get; set; }

        public List<Dictionary<string, string>> Attributes { get; set; }
    }
}