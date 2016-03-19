namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using Prism.Events;
    using Wrapper;

    public class SentenceEditorViewModel : Observable
    {
        private IEventAggregator eventAggregator;

        public SentenceEditorViewModel(
            IEventAggregator eventAggregator,
            SentenceWrapper sentence)
        {
            this.eventAggregator = eventAggregator;
            Sentence = sentence;
        }

        public SentenceWrapper Sentence { get; set; }
    }
}