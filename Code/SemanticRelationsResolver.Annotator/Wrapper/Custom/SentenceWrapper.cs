namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SentenceWrapper
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Parser))
            {
                yield return new ValidationResult("Parser is required.", new[] {"Parser"});
            }
        }
    }
}