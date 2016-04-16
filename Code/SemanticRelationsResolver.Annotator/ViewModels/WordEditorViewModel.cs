namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Windows.Input;

    using Prism.Events;

    using SemanticRelationsResolver.Annotator.Commands;
    using SemanticRelationsResolver.Annotator.Wrapper;
    using SemanticRelationsResolver.Events;

    public class WordEditorViewModel : Observable
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
            get
            {
                return wordWrapper.GetAttributeByName("form");
            }
            set
            {
                wordWrapper.SetAttributeByName("form", value);
            }
        }

        private bool WordGotFocusCommandCanExecute(object arg)
        {
            return true;
        }

        private void WordGotFocusCommandExecute(object obj)
        {
            eventAggregator.GetEvent<ChangeAttributesEditorViewModel>()
                .Publish(new ElementAttributeEditorViewModel { Attributes = wordWrapper.Attributes });
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