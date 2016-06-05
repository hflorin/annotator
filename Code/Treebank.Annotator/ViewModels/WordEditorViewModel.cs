namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Windows.Input;
    using Commands;
    using Events;
    using Prism.Events;
    using Treebank.Events;
    using Wrapper;
    using Wrapper.Base;

    public class WordEditorViewModel : Observable, IValidatableTrackingObject
    {
        private readonly IEventAggregator eventAggregator;

        private readonly WordWrapper wordWrapper;

        public WordEditorViewModel(WordWrapper wordWrapper, IEventAggregator eventAggregator)
        {
            if (wordWrapper == null)
            {
                throw new ArgumentNullException("wordWrapper");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this.wordWrapper = wordWrapper;
            this.eventAggregator = eventAggregator;
            WordChangedCommand = new DelegateCommand(WordChangedCommandExecute, WordChangedCommandCanExecute);
            WordGotFocusCommand = new DelegateCommand(WordGotFocusCommandExecute, WordGotFocusCommandCanExecute);
        }

        public ICommand WordChangedCommand { get; set; }

        public ICommand WordGotFocusCommand { get; set; }

        public string Form
        {
            get { return wordWrapper.GetAttributeByName("form"); }
            set { wordWrapper.SetAttributeByName("form", value); }
        }

        public void AcceptChanges()
        {
            wordWrapper.AcceptChanges();
        }

        public bool IsChanged
        {
            get { return wordWrapper.IsChanged; }
        }

        public void RejectChanges()
        {
            wordWrapper.RejectChanges();
        }

        public bool IsValid
        {
            get { return wordWrapper.IsValid; }
        }

        private bool WordGotFocusCommandCanExecute(object arg)
        {
            return true;
        }

        private void WordGotFocusCommandExecute(object obj)
        {
            eventAggregator.GetEvent<ChangeAttributesEditorViewModel>()
                .Publish(new ElementAttributeEditorViewModel(eventAggregator) {Attributes = wordWrapper.Attributes});
        }

        private void WordChangedCommandExecute(object obj)
        {
            eventAggregator.GetEvent<RelayoutGraphEvent>().Publish(true);
        }

        private bool WordChangedCommandCanExecute(object arg)
        {
            return true;
        }
    }
}