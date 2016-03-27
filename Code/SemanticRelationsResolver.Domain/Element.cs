namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public abstract class Element : IHasAttributes
    {
        protected Element()
        {
            Attributes = new List<Attribute>();
        }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public bool IsOptional { get; set; }

        public ICollection<Attribute> Attributes { get; set; }
    }
}