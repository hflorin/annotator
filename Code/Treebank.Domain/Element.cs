namespace Treebank.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public string GetAttributeByName(string attributeName)
        {
            var attribute = Attributes.FirstOrDefault(a => a.Name.ToLowerInvariant().Equals(attributeName.ToLowerInvariant()));

            return attribute == null ? string.Empty : attribute.Value;
        }

        public void SetAttributeByName(string attributeName, string value)
        {
            var attribute = Attributes.FirstOrDefault(a => a.Name.ToLowerInvariant().Equals(attributeName.ToLowerInvariant()));

            if (attribute != null)
            {
                attribute.Value = value;
            }
        }
    }
}