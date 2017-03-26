namespace Treebank.Domain
{
    [System.Serializable]
    public class ExceptValuesOf:Element
    {
        public string AttributeName { get; set; }
        public System.Collections.Generic.IList<string> Values { get; set; }
    }
}