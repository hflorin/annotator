namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class SentenceWrapper
    {
        public string IdValue
        {
            get { return Attributes.Single(a => a.Name.Equals("id")).Value; }
            set { Attributes.Single(a => a.Name.Equals("id")).Value = value; }
        }

        public string ContentValue
        {
            get { return Attributes.Single(a => a.Name.Equals("content")).Value; }
            set { Attributes.Single(a => a.Name.Equals("content")).Value = value; }
        }

        public string ParserValue
        {
            get { return Attributes.Single(a => a.Name.Equals("parser")).Value; }
            set { Attributes.Single(a => a.Name.Equals("parser")).Value = value; }
        }

        public string UserValue
        {
            get { return Attributes.Single(a => a.Name.Equals("user")).Value; }
            set { Attributes.Single(a => a.Name.Equals("user")).Value = value; }
        }

        public string DateValue
        {
            get { return Attributes.Single(a => a.Name.Equals("date")).Value; }
            set { Attributes.Single(a => a.Name.Equals("date")).Value = value; }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Attributes == null)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(Attributes.Single(a => a.Name.Equals("parser")).Value))
            {
                yield return new ValidationResult("Parser is required.", new[] {"Parser"});
            }
        }
    }
}