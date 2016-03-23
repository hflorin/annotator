namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class WordWrapper
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Attributes == null)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(Attributes.Single(a => a.Name.Equals("form")).Value))
            {
                yield return new ValidationResult("Form is required.", new[] { "form" });
            }

            if (Attributes.Single(a => a.Name.Equals("postag")).Value == null
                || Attributes.Single(a => a.Name.Equals("form")).Value == null)
            {
                yield return
                    new ValidationResult("A word must have a part of speech and a form", new[] { "postag", "form" });
            }
        }
    }
}