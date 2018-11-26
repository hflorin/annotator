namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Commands;
    using Wrapper;

    public class CompareSentencesViewModel : Observable
    {
        private DocumentWrapper leftSelectedDocument;

        private SentenceWrapper leftSelectedSentence;

        private DocumentWrapper rightSelectedDocument;
        private SentenceWrapper rightSelectedSentence;

        public CompareSentencesViewModel(ObservableCollection<DocumentWrapper> documentWrappers)
        {
            if (documentWrappers == null)
            {
                throw new ArgumentNullException("documentWrappers");
            }

            Documents = documentWrappers;

            OkButtonCommand = new DelegateCommand(OkButtonCommandExecute, OkButtonCommandCanExecute);
        }

        public ICommand OkButtonCommand { get; set; }

        public SentenceWrapper LeftSelectedSentence
        {
            get { return leftSelectedSentence; }
            set
            {
                leftSelectedSentence = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public SentenceWrapper RightSelectedSentence
        {
            get { return rightSelectedSentence; }
            set
            {
                rightSelectedSentence = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public DocumentWrapper LeftSelectedDocument
        {
            get { return leftSelectedDocument; }
            set
            {
                leftSelectedDocument = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public DocumentWrapper RightSelectedDocument
        {
            get { return rightSelectedDocument; }
            set
            {
                rightSelectedDocument = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DocumentWrapper> Documents { get; set; }

        private void InvalidateCommands()
        {
            ((DelegateCommand) OkButtonCommand).RaiseCanExecuteChanged();
        }

        private void OkButtonCommandExecute(object obj)
        {
        }

        private bool OkButtonCommandCanExecute(object arg)
        {
            return LeftSelectedSentence != null && RightSelectedSentence != null;
        }
    }
}