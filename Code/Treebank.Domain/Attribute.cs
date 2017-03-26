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
            ExceptedValuesOfSet = new List<ExceptValuesOf>();
        }

        public string Entity { get; set; }

        public bool IsEditable { get; set; }

        public bool IsOptional { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public ICollection<string> AllowedValuesSet { get; set; }

        public ICollection<ExceptValuesOf> ExceptedValuesOfSet { get; set; }
    }
}