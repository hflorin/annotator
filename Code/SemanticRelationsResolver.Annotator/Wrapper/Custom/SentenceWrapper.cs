namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class SentenceWrapper
    {
        public AttributeWrapper Id
        {
            get
            {
                return Attributes.Single(a => a.Name.Equals("id"));
            }
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
                return Attributes.Single(a => a.Name.Equals("content"));
            }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("content"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper Parser
        {
            get
            {
                return Attributes.Single(a => a.Name.Equals("parser"));
            }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("parser"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper User
        {
            get
            {
                return Attributes.Single(a => a.Name.Equals("user"));
            }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("user"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper Date
        {
            get
            {
                return Attributes.Single(a => a.Name.Equals("date"));
            }
            set
            {
                var oldId = Attributes.Single(a => a.Name.Equals("date"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Attributes == null)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(Attributes.Single(a => a.Name.Equals("parser")).Value))
            {
                yield return new ValidationResult("Parser is required.", new[] { "Parser" });
            }
        }
    }
}