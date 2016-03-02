namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    using SemanticRelationsResolver.Annotator.Commands;
    using SemanticRelationsResolver.Annotator.View.Services;

    public class MainViewModel : Observable
    {
        private static string currentTreebankFilepath = string.Empty;

        private readonly IOpenFileDialogService openFileDialogService;

        private readonly ISaveDialogService saveDialogService;

        public MainViewModel(ISaveDialogService saveDialogService, IOpenFileDialogService openFileDialogService)
        {
            NewTreeBankCommand = new DelegateCommand(NewTreeBankCommandExecute, NewTreeBankCommandCanExecute);
            OpenCommand = new DelegateCommand(OpenCommandExecute, OpenCommandCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecute, SaveAsCommandCanExecute);
            this.saveDialogService = saveDialogService;
            this.openFileDialogService = openFileDialogService;
        }

        public void OnClosing(CancelEventArgs cancelEventArgs)
        {
            // todo: show message box to confirm closing if there are changes in the viewmodel

            // todo:check if there are any unsaved changes, show popup to allow the user to decide, handle his response and then exit the app
        }

        #region Commands handlers

        #region File

        private bool NewTreeBankCommandCanExecute(object arg)
        {
            return true;
        }

        private void NewTreeBankCommandExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private bool OpenCommandCanExecute(object arg)
        {
            return true;
        }

        private void OpenCommandExecute(object obj)
        {
            currentTreebankFilepath = openFileDialogService.GetFileLocation(FileFilters.XmlFilesOnlyFilter);

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }
        }

        private void SaveCommandExecute(object obj)
        {
            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                currentTreebankFilepath = saveDialogService.GetSaveFileLocation(FileFilters.XmlFilesOnlyFilter);
            }

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }

            // todo: save logic
        }

        private bool SaveCommandCanExecute(object arg)
        {
            return true;
        }

        private bool SaveAsCommandCanExecute(object arg)
        {
            return true;
        }

        private void SaveAsCommandExecute(object obj)
        {
            currentTreebankFilepath = saveDialogService.GetSaveFileLocation(FileFilters.AllFilesFilter);

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }

            // todo: save as logic
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