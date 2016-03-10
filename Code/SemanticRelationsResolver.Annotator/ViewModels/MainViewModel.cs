namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using Events;
    using Mappers;
    using Prism.Events;
    using View.Services;
    using Wrapper;

    public class MainViewModel : Observable
    {
        private static string currentTreebankFilePath = string.Empty;

        private IDocumentMapper documentMapper;

        private Dictionary<string, DocumentWrapper> documentsWrappers;

        private IEventAggregator eventAggregator;

        private IOpenFileDialogService openFileDialogService;

        private ISaveDialogService saveDialogService;

        public MainViewModel(
            IEventAggregator eventAggregator,
            ISaveDialogService saveDialogService,
            IOpenFileDialogService openFileDialogService,
            IDocumentMapper documentMapper)
        {
            InitializeCommands();

            InitializeServices(eventAggregator, saveDialogService, openFileDialogService, documentMapper);

            SubscribeToEvents();

            InitializeMembers();
        }

        public ObservableCollection<DocumentWrapper> Documents { get; set; }

        public ObservableCollection<string> DocumentLoadExceptions { get; set; }

        public SentenceWrapper SelectedSentence { get; set; }

        public DocumentWrapper CurrentTreebank
        {
            get
            {
                return documentsWrappers.ContainsKey(currentTreebankFilePath)
                    ? documentsWrappers[currentTreebankFilePath]
                    : new DocumentWrapper(new Document());
            }
            set
            {
                documentsWrappers[currentTreebankFilePath] = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewTreeBankCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand LoadSentencesCommand { get; set; }

        private void InitializeMembers()
        {
            documentsWrappers = new Dictionary<string, DocumentWrapper>();
            DocumentLoadExceptions = new ObservableCollection<string>();
            Documents = new ObservableCollection<DocumentWrapper>();
        }

        private void SubscribeToEvents()
        {
            eventAggregator.GetEvent<DocumentLoadExceptionEvent>().Subscribe(OnDocumentLoadException);
        }

        private void OnDocumentLoadException(string exceptionMessage)
        {
            DocumentLoadExceptions.Add(exceptionMessage);
        }

        private void InitializeServices(
            IEventAggregator eventAggregatorArg,
            ISaveDialogService saveDialogServiceArg,
            IOpenFileDialogService openFileDialogServiceArg,
            IDocumentMapper documentMapperArg)
        {
            if (eventAggregatorArg == null)
            {
                throw new ArgumentNullException("eventAggregatorArg");
            }

            if (saveDialogServiceArg == null)
            {
                throw new ArgumentNullException("saveDialogServiceArg");
            }

            if (openFileDialogServiceArg == null)
            {
                throw new ArgumentNullException("openFileDialogServiceArg");
            }

            if (documentMapperArg == null)
            {
                throw new ArgumentNullException("documentMapperArg");
            }

            saveDialogService = saveDialogServiceArg;
            openFileDialogService = openFileDialogServiceArg;
            eventAggregator = eventAggregatorArg;
            documentMapper = documentMapperArg;
        }

        private void InitializeCommands()
        {
            NewTreeBankCommand = new DelegateCommand(NewTreeBankCommandExecute, NewTreeBankCommandCanExecute);
            OpenCommand = new DelegateCommand(OpenCommandExecute, OpenCommandCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecute, SaveAsCommandCanExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute, CloseCommandCanExecute);
            LoadSentencesCommand = new DelegateCommand(LoadSentencesCommandCanExecute, LoadSentencesCommandExecute);
        }

        private void LoadSentencesCommandCanExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private bool LoadSentencesCommandExecute(object arg)
        {
            throw new NotImplementedException();
        }

        private bool CloseCommandCanExecute(object arg)
        {
            return !string.IsNullOrWhiteSpace(currentTreebankFilePath);
        }

        private void CloseCommandExecute(object obj)
        {
            if (!documentsWrappers.ContainsKey(currentTreebankFilePath))
            {
                return;
            }

            documentsWrappers.Remove(currentTreebankFilePath);

            if (documentsWrappers.Any())
            {
                CurrentTreebank = documentsWrappers.First().Value;

                currentTreebankFilePath = CurrentTreebank.Model.FilePath;
            }

            RefreshDocumentsExplorerList();
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

            DocumentLoadExceptions.Clear();

            var documentModel = await documentMapper.Map(currentTreebankFilePath);

            //must check if the file is alredy loaded and has changes offer to save if so

            documentsWrappers[currentTreebankFilePath] = new DocumentWrapper(documentModel);
            CurrentTreebank = documentsWrappers[currentTreebankFilePath];

            RefreshDocumentsExplorerList();
        }

        private void RefreshDocumentsExplorerList()
        {
            Documents.Clear();

            foreach (var documentWrapper in documentsWrappers)
            {
                Documents.Add(documentWrapper.Value);
            }
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