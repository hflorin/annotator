namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Linq;

    public partial class DocumentWrapper
    {
        public string IdValue
        {
            get { return Attributes.Single(a => a.Name.Equals("id")).Value; }
            set { Attributes.Single(a => a.Name.Equals("id")).Value = value; }
        }
    }
}