namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class WordWrapper
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PartOfSpeech == null || Form == null)
            {
                yield return
                    new ValidationResult("A word must have a part of speech and a form", new[] {"PartOfSpeech", "Form"})
                    ;
            }
        }
    }
}