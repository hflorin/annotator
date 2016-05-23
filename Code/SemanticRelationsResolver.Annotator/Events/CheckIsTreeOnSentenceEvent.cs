namespace SemanticRelationsResolver.Annotator.Events
{
    using Prism.Events;
    using Wrapper;

    public class CheckIsTreeOnSentenceEvent : PubSubEvent<SentenceWrapper>
    {
    }
}