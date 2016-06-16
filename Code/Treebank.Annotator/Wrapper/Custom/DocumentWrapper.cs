namespace Treebank.Annotator.Wrapper
{
    using System.Linq;

    public partial class DocumentWrapper
    {
        public string IdValue
        {
            get
            {
                var attribute = Attributes.FirstOrDefault(a => a.Name.Equals("id"));

                if (attribute != null)
                {
                    return attribute.Value;
                }

                return "Undefined";
            }

            set
            {
                var attribute = Attributes.FirstOrDefault(a => a.Name.Equals("id"));

                if (attribute != null)
                {
                    attribute.Value = value;
                }
            }
        }
    }
}