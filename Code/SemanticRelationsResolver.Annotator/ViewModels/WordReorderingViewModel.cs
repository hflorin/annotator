namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Windows.Input;
    using Commands;
    using Wrapper;

    public class WordReorderingViewModel : Observable
    {
        public WordReorderingViewModel(SentenceWrapper sentenceWrapper)
        {
            if (sentenceWrapper == null)
            {
                throw new ArgumentNullException("sentenceWrapper");
            }

            InitializeCommands();
            Sentence = sentenceWrapper;
        }

        public ICommand MoveUp { get; set; }
        public ICommand MoveDown { get; set; }

        public SentenceWrapper Sentence { get; set; }

        private void InitializeCommands()
        {
            MoveUp = new DelegateCommand(MoveUpCommandExecute, MoveUpCommandCanExecute);
            MoveDown = new DelegateCommand(MoveDownCommandExecute, MoveDownCommandCanExecute);
        }

        private void MoveDownCommandExecute(object obj)
        {
        }

        private bool MoveDownCommandCanExecute(object arg)
        {
            return true;
        }

        private void MoveUpCommandExecute(object obj)
        {
        }

        private bool MoveUpCommandCanExecute(object arg)
        {
            return true;
        }
    }
}