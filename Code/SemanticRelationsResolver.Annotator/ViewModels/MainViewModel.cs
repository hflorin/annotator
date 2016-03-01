namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    using SemanticRelationsResolver.Annotator.Commands;

    public class MainViewModel : Observable
    {
        private const string AllowedTreebankFileFormatsFilter = "XML files (*.xml)|*.xml";

        private const string AllFilesFilter = "All files (*.*)|*.*";

        private static string currentTreebankFilepath = string.Empty;

        public MainViewModel()
        {
            NewTreeBankCommand = new DelegateCommand(NewTreeBankCommandExecute, NewTreeBankCommandCanExecute);
            OpenCommand = new DelegateCommand(OpenCommandExecute, OpenCommandCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecute, SaveAsCommandCanExecute);
        }

        public void OnClosing(CancelEventArgs cancelEventArgs)
        {
            // todo: show message box to confirm closing if there are changes in the viewmodel
        }

        #region Commands handlers

        #region File

        private bool NewTreeBankCommandCanExecute(object arg)
        {
            throw new NotImplementedException();
        }

        private void NewTreeBankCommandExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private bool OpenCommandCanExecute(object arg)
        {
            throw new NotImplementedException();
        }

        private void OpenCommandExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void SaveCommandExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private bool SaveCommandCanExecute(object arg)
        {
            throw new NotImplementedException();
        }

        private bool SaveAsCommandCanExecute(object arg)
        {
            throw new NotImplementedException();
        }

        private void SaveAsCommandExecute(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Commands properties

        #region File

        public ICommand NewTreeBankCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        #endregion

        #endregion
    }
}