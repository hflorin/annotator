namespace Treebank.Annotator.Graph
{
    using Wrapper;

    public class OrderedWordVertex : WordVertex
    {
        public OrderedWordVertex(WordWrapper wordWrapper, string vertexValueAttribute)
            : base(wordWrapper, vertexValueAttribute)
        {
        }

        public bool IsRight { get; set; }
        public bool IsLeft { get; set; }
    }
}