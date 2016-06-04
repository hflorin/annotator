namespace Treebank.Domain
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public abstract class Element : IEntity
    {
        protected Element()
        {
            Attributes = new List<Domain.Attribute>();
        }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public bool IsOptional { get; set; }

        public string Entity { get; set; }

        public ICollection<Domain.Attribute> Attributes { get; set; }
    }
}