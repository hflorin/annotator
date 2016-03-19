namespace SemanticRelationsResolver.Annotator.Graph
{
    using GraphX.PCL.Logic.Models;
    using QuickGraph;

    public class SentenceGxLogicCore :
        GXLogicCore<WordVertex, WordEdge, BidirectionalGraph<WordVertex, WordEdge>>
    {
    }
}