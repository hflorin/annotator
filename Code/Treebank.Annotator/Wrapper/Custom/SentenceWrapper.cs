﻿namespace Treebank.Annotator.Wrapper
{
    using System.Linq;
    using Domain;

    public partial class SentenceWrapper
    {
        public AttributeWrapper Id
        {
            get { return Attributes.FirstOrDefault(a => a.Name.Equals("id")); }

            set
            {
                var oldId = Attributes.FirstOrDefault(a => a.Name.Equals("id"));
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

                return Attributes.FirstOrDefault(a => a.Name.Equals("content"));
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
            get { return Attributes.FirstOrDefault(a => a.Name.Equals("parser")); }

            set
            {
                var oldId = Attributes.FirstOrDefault(a => a.Name.Equals("parser"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper User
        {
            get { return Attributes.FirstOrDefault(a => a.Name.Equals("user")); }

            set
            {
                var oldId = Attributes.FirstOrDefault(a => a.Name.Equals("user"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public AttributeWrapper Date
        {
            get { return Attributes.FirstOrDefault(a => a.Name.Equals("date")); }

            set
            {
                var oldId = Attributes.FirstOrDefault(a => a.Name.Equals("date"));
                Attributes.Remove(oldId);
                Attributes.Add(value);
            }
        }

        public bool IsTree { get; set; }
        //    }
        //        yield break;
        //    {
        //    if (Attributes == null)
        //{

        //public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)

        //    var attribute = Attributes.FirstOrDefault(a => (a.Name != null) && a.Name.Equals("parser"));

        //    if (attribute == null || string.IsNullOrWhiteSpace(attribute.Value))
        //    {
        //        yield return new ValidationResult("Parser is required.", new[] { "Parser" });
        //    }
        //}
    }
}