namespace Treebank.Annotator.Graph
{
    public class OrderedWordEdge : WordEdge
    {
        public OrderedWordEdge(WordVertex source, WordVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public bool IsRight { get; set; }
        public bool IsLeft { get; set; }
    }
}