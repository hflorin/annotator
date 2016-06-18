namespace Treebank.Annotator.Wrapper
{
    using System.Linq;

    public partial class WordWrapper
    {
        public string Form
        {
            get { return GetAttributeByName("form"); }

            set { SetAttributeByName("form", value); }
        }

        public int Id
        {
            get { return int.Parse(GetAttributeByName("id")); }

            set { SetAttributeByName("id", value.ToString()); }
        }

        public int HeadWordId
        {
            get { return int.Parse(GetAttributeByName("head")); }

            set { SetAttributeByName("head", value.ToString()); }
        }

        //public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Attributes == null)
        //    {
        //        yield break;
        //    }

        //    var firstOrDefault = Attributes.FirstOrDefault(a => a.Name.Equals("form"));
        //    if (firstOrDefault != null && string.IsNullOrWhiteSpace(firstOrDefault.Value))
        //    {
        //        yield return new ValidationResult("Form is required.", new[] { "form" });
        //    }

        //    if (string.IsNullOrEmpty(GetAttributeByName("postag")) || string.IsNullOrEmpty(GetAttributeByName("form")))
        //    {
        //        yield return
        //            new ValidationResult("A word must have a part of speech and a form", new[] { "postag", "form" });
        //    }
        //}

        public string GetAttributeByName(string attributeName)
        {
            var firstOrDefault =
                Attributes.FirstOrDefault(a => a.Name.ToLowerInvariant().Equals(attributeName.ToLowerInvariant()));
            if (firstOrDefault != null)
            {
                return firstOrDefault.Value;
            }

            return string.Empty;
        }

        public void SetAttributeByName(string attributeName, string value)
        {
            var firstOrDefault =
                Attributes.FirstOrDefault(a => a.Name.ToLowerInvariant().Equals(attributeName.ToLowerInvariant()));
            if (firstOrDefault != null)
            {
                firstOrDefault.Value = value;
                OnPropertyChanged("Attributes");
            }
        }
    }
}