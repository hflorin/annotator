namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using Commands;
    using Mappers;
    using Prism.Events;
    using View.Services;
    using Wrapper;

    public class MainViewModel : Observable
    {
        private static string currentTreebankFilePath = string.Empty;

        private readonly IDocumentMapper documentMapper;

        private readonly IEventAggregator eventAggregator;

        private readonly IOpenFileDialogService openFileDialogService;

        private readonly ISaveDialogService saveDialogService;

        private readonly Dictionary<string, DocumentWrapper> treebankWrappers;

        private string currentTreebankWrapperId;

        public MainViewModel(
            IEventAggregator eventAggregator,
            ISaveDialogService saveDialogService,
            IOpenFileDialogService openFileDialogService,
            IDocumentMapper documentMapper)
        {
            InitializeCommands();

            CheckArgumentsForNull(eventAggregator, saveDialogService, openFileDialogService, documentMapper);

            this.saveDialogService = saveDialogService;
            this.openFileDialogService = openFileDialogService;
            this.eventAggregator = eventAggregator;
            this.documentMapper = documentMapper;

            treebankWrappers = new Dictionary<string, DocumentWrapper>();
            TreebankIds = new ObservableCollection<string>();
        }

        public ObservableCollection<string> TreebankIds { get; set; }

        public DocumentWrapper CurrentTreebank
        {
            get { return treebankWrappers[currentTreebankWrapperId]; }
            set
            {
                treebankWrappers[currentTreebankWrapperId] = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewTreeBankCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        private static void CheckArgumentsForNull(
            IEventAggregator eventAggregator,
            ISaveDialogService saveDialogService,
            IOpenFileDialogService openFileDialogService,
            IDocumentMapper documentMapper)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            if (saveDialogService == null)
            {
                throw new ArgumentNullException("saveDialogService");
            }

            if (openFileDialogService == null)
            {
                throw new ArgumentNullException("openFileDialogService");
            }

            if (documentMapper == null)
            {
                throw new ArgumentNullException("documentMapper");
            }
        }

        private void InitializeCommands()
        {
            NewTreeBankCommand = new DelegateCommand(NewTreeBankCommandExecute, NewTreeBankCommandCanExecute);
            OpenCommand = new DelegateCommand(OpenCommandExecute, OpenCommandCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecute, SaveAsCommandCanExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute, CloseCommandCanExecute);
        }

        private bool CloseCommandCanExecute(object arg)
        {
            return !string.IsNullOrWhiteSpace(currentTreebankWrapperId);
        }

        private void CloseCommandExecute(object obj)
        {
            if (treebankWrappers.ContainsKey(currentTreebankWrapperId))
            {
                treebankWrappers.Remove(currentTreebankWrapperId);
                TreebankIds.Remove(currentTreebankWrapperId);
            }

            currentTreebankWrapperId = string.Empty;
        }

        public void OnClosing(CancelEventArgs cancelEventArgs)
        {
            // todo: show message box to confirm closing if there are changes in the viewmodel

            // todo:check if there are any unsaved changes, show popup to allow the user to decide, handle his response and then exit the app
        }

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

        private async void OpenCommandExecute(object obj)
        {
            currentTreebankFilePath = openFileDialogService.GetFileLocation(FileFilters.XmlFilesOnlyFilter);

            if (string.IsNullOrWhiteSpace(currentTreebankFilePath))
            {
                return;
            }

            var treebankModel = await documentMapper.Map(currentTreebankFilePath);

            if (treebankWrappers.ContainsKey(treebankModel.Id))
            {
                return;
            }

            treebankWrappers[treebankModel.Id] = new DocumentWrapper(treebankModel);
            currentTreebankWrapperId = treebankModel.Id;
            CurrentTreebank = treebankWrappers[currentTreebankWrapperId];
            TreebankIds.Add(treebankModel.Id);
        }

        private void SaveCommandExecute(object obj)
        {
            if (string.IsNullOrWhiteSpace(currentTreebankFilePath))
            {
                currentTreebankFilePath = saveDialogService.GetSaveFileLocation(FileFilters.XmlFilesOnlyFilter);
            }

            if (string.IsNullOrWhiteSpace(currentTreebankFilePath))
            {
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
            currentTreebankFilePath = saveDialogService.GetSaveFileLocation(FileFilters.AllFilesFilter);

            if (string.IsNullOrWhiteSpace(currentTreebankFilePath))
            {
            }

            // todo: save as logic
        }
    }
}