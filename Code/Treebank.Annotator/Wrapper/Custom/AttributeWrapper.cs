namespace Treebank.Annotator.Wrapper
{
    using System.Linq;

    public partial class AttributeWrapper
    {
        public bool HasAllowedValuesSet
        {
            get { return AllowedValuesSet.Any(); }
        }
    }
}