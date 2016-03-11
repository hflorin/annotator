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
        private IDocumentMapper documentMapper;

        private Dictionary<string, DocumentWrapper> documentsWrappers;

        private IEventAggregator eventAggregator;

        private IOpenFileDialogService openFileDialogService;

        private ISaveDialogService saveDialogService;

        private DocumentWrapper selectedDocument;

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

        public DocumentWrapper SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                selectedDocument = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewTreeBankCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand CloseCommand { get; set; }

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
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand) NewTreeBankCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) OpenCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) SaveAsCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) CloseCommand).RaiseCanExecuteChanged();
        }

        private bool CloseCommandCanExecute(object arg)
        {
            return SelectedDocument != null;
        }

        private void CloseCommandExecute(object obj)
        {
            if (SelectedDocument == null)
            {
                return;
            }

            documentsWrappers.Remove(SelectedDocument.Model.FilePath);
            SelectedDocument = null;

            if (documentsWrappers.Any())
            {
                SelectedDocument = documentsWrappers.First().Value;
            }

            RefreshDocumentsExplorerList();
            InvalidateCommands();
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
            Documents.Add(new DocumentWrapper(new Document
            {
                Identifier = "Treebank" + Documents.Count
            }));
        }

        private bool OpenCommandCanExecute(object arg)
        {
            return true;
        }

        private async void OpenCommandExecute(object obj)
        {
            var documentFilePath = openFileDialogService.GetFileLocation(FileFilters.XmlFilesOnlyFilter);

            if (string.IsNullOrWhiteSpace(documentFilePath))
            {
                return;
            }

            DocumentLoadExceptions.Clear();

            var documentModel = await documentMapper.Map(documentFilePath);

            //must check if the file is alredy loaded and has changes offer to save if so

            documentsWrappers[documentFilePath] = new DocumentWrapper(documentModel);

            RefreshDocumentsExplorerList();
            InvalidateCommands();
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
            var documentFilePath = selectedDocument !=null? selectedDocument.Model.FilePath:saveDialogService.GetSaveFileLocation(FileFilters.XmlFilesOnlyFilter);

            if (string.IsNullOrWhiteSpace(documentFilePath))
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
            var documentFilePath = saveDialogService.GetSaveFileLocation(FileFilters.AllFilesFilter);

            if (string.IsNullOrWhiteSpace(documentFilePath))
            {
                return;
            }

            // todo: save as logic
        }
    }
}