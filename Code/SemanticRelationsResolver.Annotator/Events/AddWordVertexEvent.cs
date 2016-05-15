namespace SemanticRelationsResolver.Events
{
    using Annotator.Wrapper;
    using Prism.Events;

    public class AddWordVertexEvent : PubSubEvent<WordWrapper>
    {
    }
}