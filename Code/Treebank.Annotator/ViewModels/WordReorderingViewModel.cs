namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Windows.Input;

    using Treebank.Annotator.Commands;
    using Treebank.Annotator.Wrapper;

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

        public ICommand MoveUpCommand { get; set; }

        public ICommand MoveDownCommand { get; set; }

        public SentenceWrapper Sentence { get; set; }

        public WordWrapper SelectedWord { get; set; }

        private void InitializeCommands()
        {
            MoveUpCommand = new DelegateCommand(MoveUpCommandExecute, MoveUpCommandCanExecute);
            MoveDownCommand = new DelegateCommand(MoveDownCommandExecute, MoveDownCommandCanExecute);
        }

        private void MoveDownCommandExecute(object obj)
        {
            if (SelectedWord == null)
            {
                return;
            }

            var numberOfElements = Sentence.Words.Count;
            var selectedWordIndex = Sentence.Words.IndexOf(SelectedWord);
            if (selectedWordIndex < numberOfElements - 1)
            {
                var tempIdCurrent = Sentence.Words[selectedWordIndex].GetAttributeByName("id");
                var tempIdNext = Sentence.Words[selectedWordIndex + 1].GetAttributeByName("id");
                Sentence.Words[selectedWordIndex].SetAttributeByName("id", tempIdNext);
                Sentence.Words[selectedWordIndex + 1].SetAttributeByName("id", tempIdCurrent);

                var temp = Sentence.Words[selectedWordIndex];
                Sentence.Words[selectedWordIndex] = Sentence.Words[selectedWordIndex + 1];
                Sentence.Words[selectedWordIndex + 1] = temp;

                SwitchHeadWordIds(tempIdCurrent, tempIdNext);
                Sentence.Words.AcceptChanges();

                SelectedWord = Sentence.Words[selectedWordIndex + 1];
            }
        }

        private void SwitchHeadWordIds(string firstHeadWordId, string secondHeadWordId)
        {
            for (var i = 0; i < Sentence.Words.Count; i++)
            {
                var headWordId = Sentence.Words[i].GetAttributeByName("head");
                if (headWordId == firstHeadWordId)
                {
                    Sentence.Words[i].SetAttributeByName("head", secondHeadWordId);
                }

                if (headWordId == secondHeadWordId)
                {
                    Sentence.Words[i].SetAttributeByName("head", firstHeadWordId);
                }
            }
        }

        private bool MoveDownCommandCanExecute(object arg)
        {
            return true;
        }

        private void MoveUpCommandExecute(object obj)
        {
            if (SelectedWord == null)
            {
                return;
            }

            var selectedWordIndex = Sentence.Words.IndexOf(SelectedWord);
            if (selectedWordIndex > 0)
            {
                var tempIdCurrent = Sentence.Words[selectedWordIndex].GetAttributeByName("id");
                var tempIdNext = Sentence.Words[selectedWordIndex - 1].GetAttributeByName("id");
                Sentence.Words[selectedWordIndex].SetAttributeByName("id", tempIdNext);
                Sentence.Words[selectedWordIndex - 1].SetAttributeByName("id", tempIdCurrent);

                var temp = Sentence.Words[selectedWordIndex];
                Sentence.Words[selectedWordIndex] = Sentence.Words[selectedWordIndex - 1];
                Sentence.Words[selectedWordIndex - 1] = temp;

                SwitchHeadWordIds(tempIdCurrent, tempIdNext);
                Sentence.Words.AcceptChanges();

                SelectedWord = Sentence.Words[selectedWordIndex - 1];
            }
        }

        private bool MoveUpCommandCanExecute(object arg)
        {
            return true;
        }
    }
}