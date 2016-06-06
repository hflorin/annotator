namespace Treebank.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Treebank.Domain;

    public partial class SentenceWrapper
    {
        public AttributeWrapper Id
        {
            get { return Attributes.Single(a => a.Name.Equals("id")); }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("id"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper Content
        {
            get
            {
                var contentAttribute = Attributes.FirstOrDefault(a => a.Name.Equals("content"));

                if (contentAttribute == null)
                {
                    contentAttribute =
                        new AttributeWrapper(
                            new Attribute
                                {
                                    Value = string.Empty,
                                    DisplayName = "Content",
                                    Entity = "attribute",
                                    Name = "content"
                                });

                    Attributes.Add(contentAttribute);
                }

                return Attributes.Single(a => a.Name.Equals("content"));
            }

            set
            {
                var oldIdAttribute = Attributes.FirstOrDefault(a => a.Name.Equals("content"));
                if (oldIdAttribute != null)
                {
                    Attributes.Remove(oldIdAttribute);
                }

                Attributes.Add(value);
            }
        }

        public AttributeWrapper Parser
        {
            get { return Attributes.Single(a => a.Name.Equals("parser")); }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("parser"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper User
        {
            get { return Attributes.Single(a => a.Name.Equals("user")); }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("user"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper Date
        {
            get { return Attributes.Single(a => a.Name.Equals("date")); }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("date"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public bool IsTree { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Attributes == null)
            {
                yield break;
            }

            var attribute = Attributes.FirstOrDefault(a => (a.Name != null) && a.Name.Equals("parser"));

            if (attribute == null || string.IsNullOrWhiteSpace(attribute.Value))
            {
                yield return new ValidationResult("Parser is required.", new[] { "Parser" });
            }
        }
    }
}