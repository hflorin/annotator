namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class WordWrapper
    {
        public string Form
        {
            get { return GetAttributeByName("form"); }
            set
            {
                SetAttributeByName("form", value);
                OnPropertyChanged("Attributes");
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Attributes == null)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(Attributes.Single(a => a.Name.Equals("form")).Value))
            {
                yield return new ValidationResult("Form is required.", new[] {"form"});
            }

            if (string.IsNullOrEmpty(GetAttributeByName("postag"))
                || string.IsNullOrEmpty(GetAttributeByName("form")))
            {
                yield return
                    new ValidationResult("A word must have a part of speech and a form", new[] {"postag", "form"});
            }
        }

        public string GetAttributeByName(string attributeName)
        {
            return Attributes.Single(a => a.Name.ToLowerInvariant().Equals(attributeName.ToLowerInvariant())).Value;
        }

        public void SetAttributeByName(string attributeName, string value)
        {
            Attributes.Single(a => a.Name.ToLowerInvariant().Equals(attributeName.ToLowerInvariant())).Value = value;
        }
    }
}