namespace SemanticRelationsResolver.Annotator.Graph
{
    using GraphX.PCL.Common.Models;

    public class WordVertex : VertexBase
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}