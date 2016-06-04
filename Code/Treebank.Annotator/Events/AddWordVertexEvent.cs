namespace Treebank.Annotator.Events
{
    using Prism.Events;
    using Wrapper;

    public class AddWordVertexEvent : PubSubEvent<WordWrapper>
    {
    }
}