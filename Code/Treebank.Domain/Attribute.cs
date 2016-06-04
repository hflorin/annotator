namespace Treebank.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Attribute : IEntity
    {
        public Attribute()
        {
            AllowedValuesSet = new List<string>();
        }

        public string Entity { get; set; }

        public bool IsEditable { get; set; }

        public bool IsOptional { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public ICollection<string> AllowedValuesSet { get; set; }
    }
}