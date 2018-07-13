namespace Treebank.Annotator.Events
{
    using Prism.Events;
    using Wrapper;

    public class UpdateSentenceEvent : PubSubEvent<UpdateSentenceEventData>
    {
    }

    public class UpdateSentenceEventData
    {
        public SentenceWrapper Sentence { get; set; }
        public string DocumentId { get; set; }
    }
}