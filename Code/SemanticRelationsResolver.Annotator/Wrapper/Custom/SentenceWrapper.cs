namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class SentenceWrapper
    {
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