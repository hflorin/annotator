namespace Treebank.Annotator.Graph
{
    using GraphX.PCL.Common.Models;

    public class WordEdge : EdgeBase<WordVertex>
    {
        public WordEdge(WordVertex source, WordVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public WordEdge()
            : base(null, null, 1)
        {
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}