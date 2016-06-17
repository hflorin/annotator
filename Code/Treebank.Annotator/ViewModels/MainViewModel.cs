namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;
    using System.Windows.Input;

    using GraphX.PCL.Logic.Helpers;

    using Prism.Events;

    using Treebank.Annotator.Commands;
    using Treebank.Annotator.Events;
    using Treebank.Annotator.Graph.Algos;
    using Treebank.Annotator.View;
    using Treebank.Annotator.View.Services;
    using Treebank.Annotator.Wrapper;
    using Treebank.Annotator.Wrapper.Base;
    using Treebank.Domain;
    using Treebank.Events;
    using Treebank.Mappers;
    using Treebank.Mappers.Configuration;

    using Attribute = Treebank.Domain.Attribute;

    public class MainViewModel : Observable
    {
        private SentenceEditorView activeSentenceEditorView;

        private IAppConfig appConfig;

        private IAppConfigMapper appConfigMapper;

        private string currentStatus;

        private IDocumentMapper documentMapper;

        private Dictionary<string, DocumentWrapper> documentsWrappers;

        private IEventAggregator eventAggregator;

        private IOpenFileDialogService openFileDialogService;

        private ISaveDialogService saveDialogService;

        private DocumentWrapper selectedDocument;

        private ElementAttributeEditorViewModel selectedElementAttributeEditorViewModel;

        private SentenceWrapper sentence;

        private ObservableCollection<SentenceEditorView> sentenceEditViewModels;

        private IShowInfoMessage showInfoMessage;

        public MainViewModel(
            IEventAggregator eventAggregator, 
            ISaveDialogService saveDialogService, 
            IOpenFileDialogService openFileDialogService, 
            IDocumentMapper documentMapper, 
            IAppConfigMapper appConfigMapper, 
            IShowInfoMessage showInfoMessage)
        {
            InitializeCommands();

            InitializeServices(
                eventAggregator, 
                saveDialogService, 
                openFileDialogService, 
                documentMapper, 
                appConfigMapper, 
                showInfoMessage);

            SubscribeToEvents();

            InitializeMembers();
        }

        public ObservableCollection<DocumentWrapper> Documents { get; set; }

        public ObservableCollection<string> DocumentLoadExceptions { get; set; }

        public string CurrentStatus
        {
            get
            {
                return currentStatus;
            }

            set
            {
                currentStatus = value;
                OnPropertyChanged();
            }
        }

        public SentenceWrapper SelectedSentence
        {
            get
            {
                return sentence;
            }

            set
            {
                sentence = value;

                ((DelegateCommand)EditSentenceCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)EditWordOrderCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)AddSentenceCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)DeleteSentenceCommand).RaiseCanExecuteChanged();

                OnPropertyChanged();
            }
        }

        public DocumentWrapper SelectedDocument
        {
            get
            {
                return selectedDocument;
            }

            set
            {
                selectedDocument = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SentenceEditorView> SentenceEditViews
        {
            get
            {
                return sentenceEditViewModels;
            }

            set
            {
                sentenceEditViewModels = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewTreeBankCommand { get; set; }

        public ICommand BindAttributesCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand EditSentenceCommand { get; set; }

        public ICommand AddSentenceCommand { get; set; }

        public ICommand DeleteSentenceCommand { get; set; }

        public ICommand EditWordOrderCommand { get; set; }

        public ICommand SelectedSentenceChangedCommand { get; set; }

        public SentenceEditorView ActiveSentenceEditorView
        {
            get
            {
                return activeSentenceEditorView;
            }

            set
            {
                activeSentenceEditorView = value;
                OnPropertyChanged();
            }
        }

        public ElementAttributeEditorViewModel SelectedElementAttributeEditorViewModel
        {
            get
            {
                return selectedElementAttributeEditorViewModel;
            }

            set
            {
                selectedElementAttributeEditorViewModel = value;
                OnPropertyChanged();
            }
        }

        public void OnClosing(CancelEventArgs cancelEventArgs)
        {
            // todo: show message box to confirm closing if there are changes in the viewmodel

            // todo:check if there are any unsaved changes, show popup to allow the user to decide, handle his response and then exit the app
        }

        private void InitializeMembers()
        {
            documentsWrappers = new Dictionary<string, DocumentWrapper>();
            DocumentLoadExceptions = new ObservableCollection<string>();
            Documents = new ObservableCollection<DocumentWrapper>();
            sentenceEditViewModels = new ObservableCollection<SentenceEditorView>();
        }

        private void SubscribeToEvents()
        {
            eventAggregator.GetEvent<ValidationExceptionEvent>().Subscribe(OnDocumentLoadException);
            eventAggregator.GetEvent<StatusNotificationEvent>().Subscribe(OnStatusNotification);
            eventAggregator.GetEvent<ChangeAttributesEditorViewModel>().Subscribe(OnAttributesChanged);
            eventAggregator.GetEvent<CheckIsTreeOnSentenceEvent>().Subscribe(OnCheckIsTreeOnSentence);
        }

        private void OnCheckIsTreeOnSentence(SentenceWrapper sentenceWrapper)
        {
            if (sentenceWrapper == null)
            {
                return;
            }

            var validationResult = new CheckGraphResult();
            sentenceWrapper.IsTree =
                GraphOperations.GetGraph(sentenceWrapper, appConfig.Definitions.First(), eventAggregator)
                    .IsTree(validationResult);

            if (!sentenceWrapper.IsTree)
            {
                foreach (var disconnectedWordId in validationResult.DisconnectedWordIds)
                {
                    eventAggregator.GetEvent<ValidationExceptionEvent>()
                        .Publish(
                            string.Format(
                                "The word id: {0}, in sentence id: {1}, is not connected to another word.", 
                                disconnectedWordId, 
                                sentenceWrapper.Id.Value));
                }

                foreach (var cycle in validationResult.Cycles)
                {
                    eventAggregator.GetEvent<ValidationExceptionEvent>()
                        .Publish(
                            string.Format(
                                "The sentence with id {0} has cycle: {1}", 
                                sentence.Id.Value, 
                                string.Join(",", cycle)));
                }

                if (validationResult.DisconnectedWordIds.Any() || validationResult.Cycles.Any())
                {
                    eventAggregator.GetEvent<StatusNotificationEvent>()
                        .Publish("Please check warnings in the Output panel.");
                }
            }
        }

        private void OnAttributesChanged(ElementAttributeEditorViewModel newViewModel)
        {
            SelectedElementAttributeEditorViewModel = newViewModel;
        }

        private void OnStatusNotification(string statusNotification)
        {
            CurrentStatus = statusNotification;
        }

        private void OnDocumentLoadException(string exceptionMessage)
        {
            DocumentLoadExceptions.Add(exceptionMessage);
        }

        private void InitializeServices(
            IEventAggregator eventAggregatorArg,
            ISaveDialogService saveDialogServiceArg,
            IOpenFileDialogService openFileDialogServiceArg,
            IDocumentMapper documentMapperArg,
            IAppConfigMapper configMapper,
            IShowInfoMessage showMessage)
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

            if (configMapper == null)
            {
                throw new ArgumentNullException("configMapper");
            }

            if (showMessage == null)
            {
                throw new ArgumentNullException("showMessage");
            }

            showInfoMessage = showMessage;
            saveDialogService = saveDialogServiceArg;
            openFileDialogService = openFileDialogServiceArg;
            eventAggregator = eventAggregatorArg;
            documentMapper = documentMapperArg;
            appConfigMapper = configMapper;
            if (appConfig == null)
            {
                appConfig = appConfigMapper.Map(ConfigurationManager.AppSettings["configurationFilePath"]).Result;
            }
        }

        private void InitializeCommands()
        {
            NewTreeBankCommand = new DelegateCommand(NewTreeBankCommandExecute, NewTreeBankCommandCanExecute);
            OpenCommand = new DelegateCommand(OpenCommandExecute, OpenCommandCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecute, SaveAsCommandCanExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute, CloseCommandCanExecute);
            EditSentenceCommand = new DelegateCommand(EditSentenceCommandExecute, EditSentenceCommandCanExecute);
            SelectedSentenceChangedCommand = new DelegateCommand(
                SelectedSentenceChangedCommandExecute, 
                SelectedSentenceChangedCommandCanExecute);
            EditWordOrderCommand = new DelegateCommand(EditWordOrderCommandExecute, EditWordOrderCommandCanExecute);
            AddSentenceCommand = new DelegateCommand(AddSentenceCommandExecute, AddSentenceCommandCanExecute);
            DeleteSentenceCommand = new DelegateCommand(DeleteSentenceCommandExecute, DeleteSentenceCommandCanExecute);
            BindAttributesCommand = new DelegateCommand(BindAttributesCommandExecute, BindAttributesCommandCanExecute);
        }

        private void BindAttributesCommandExecute(object obj)
        {
            if (SelectedDocument != null)
            {
                eventAggregator.GetEvent<ChangeAttributesEditorViewModel>()
                    .Publish(new ElementAttributeEditorViewModel(eventAggregator, Guid.Empty)
                    {
                        Attributes = SelectedDocument.Attributes
                    });
            }
        }

        private bool BindAttributesCommandCanExecute(object arg)
        {
            return SelectedDocument != null;
        }

        private bool DeleteSentenceCommandCanExecute(object arg)
        {
            return (SelectedDocument != null) && (SelectedSentence != null);
        }

        private void DeleteSentenceCommandExecute(object obj)
        {
            if ((SelectedDocument == null) || (SelectedSentence == null))
            {
                return;
            }

            if (SelectedDocument.Sentences.Count <= 1)
            {
                CloseCommandExecute(null);
                return;
            }

            if ((SelectedDocument != null) && (SelectedSentence != null))
            {
                var sentenceToRemoveIndex = SelectedDocument.Sentences.IndexOf(SelectedSentence);

                SelectedSentence =
                    SelectedDocument.Sentences.Except(new List<SentenceWrapper> { SelectedSentence }).FirstOrDefault();
                SelectedDocument.Sentences.RemoveAt(sentenceToRemoveIndex);
            }
        }

        private bool AddSentenceCommandCanExecute(object arg)
        {
            return SelectedDocument != null;
        }

        private void AddSentenceCommandExecute(object obj)
        {
            if (SelectedDocument != null)
            {
                var sentencePrototype = appConfig.Elements.OfType<Sentence>().FirstOrDefault();
                var wordPrototype = appConfig.Elements.OfType<Word>().FirstOrDefault();
                var configuration = appConfig.Definitions.FirstOrDefault();

                if (configuration == null)
                {
                    eventAggregator.GetEvent<StatusNotificationEvent>().Publish("Must define a configuration in the configuration file before adding a sentence");
                    return;
                }

                var inputDialog = new InputDialog("Enter sentence");

                if (inputDialog.ShowDialog().GetValueOrDefault())
                {
                    string sentenceContent = inputDialog.Value;

                    if (sentencePrototype != null)
                    {
                        var sentenceClone = ObjectCopier.Clone(sentencePrototype);

                        sentenceClone.SetAttributeByName("date", DateTime.Now.ToString("d"));
                        sentenceClone.SetAttributeByName("id", (SelectedDocument.Sentences.Count + 1).ToString());
                        var newSentence = new SentenceWrapper(sentenceClone)
                                              {
                                                  IsOptional = false,
                                                  Content =
                                                      new AttributeWrapper(
                                                      new Attribute
                                                          {
                                                              Name = "content",
                                                              DisplayName =
                                                                  "Content",
                                                              Value =
                                                                  sentenceContent,
                                                              IsOptional = true,
                                                              IsEditable = false
                                                          })
                                              };

                        var words = sentenceContent.Split(' ');

                        for (var i = 0; i < words.Length; i++)
                        {
                            var wordContent = words[i];
                            var newWord = ObjectCopier.Clone(wordPrototype);
                            newWord.Value = wordContent;

                            newWord.SetAttributeByName(configuration.Vertex.ToAttributeName, (i + 1).ToString());
                            newWord.SetAttributeByName(configuration.Vertex.FromAttributeName, "-1");

                            newWord.Attributes.Add(
                                new Attribute
                                    {
                                        Name = "content",
                                        DisplayName = "Content",
                                        Value = wordContent,
                                        IsOptional = true,
                                        IsEditable = false
                                    });

                            if (wordPrototype != null)
                            {
                                var attribute =
                                    newWord.Attributes.FirstOrDefault(
                                        a => a.Name == configuration.Vertex.LabelAttributeName);

                                if (attribute != null)
                                {
                                    attribute.Value = wordContent;
                                }
                            }

                            var newWordWrapper = new WordWrapper(newWord);

                            newSentence.Words.Add(newWordWrapper);
                        }

                        newSentence.Attributes.ForEach(
                            a =>
                                {
                                    a.IsOptional = false;
                                    a.IsEditable = true;
                                });

                        SelectedDocument.Sentences.Add(newSentence);
                        SelectedSentence = newSentence;
                    }
                }
            }
        }

        private bool EditWordOrderCommandCanExecute(object arg)
        {
            return SelectedSentence != null;
        }

        private void EditWordOrderCommandExecute(object obj)
        {
            if (SelectedSentence == null)
            {
                return;
            }

            var wordReorderingWindow = new WordReorderingWindow(new WordReorderingViewModel(SelectedSentence));
            if (wordReorderingWindow.ShowDialog().GetValueOrDefault())
            {
                eventAggregator.GetEvent<GenerateGraphEvent>().Publish(ActiveSentenceEditorView.ViewId);
                eventAggregator.GetEvent<ZoomToFillEvent>().Publish(ActiveSentenceEditorView.ViewId);
            }
        }

        private bool SelectedSentenceChangedCommandCanExecute(object arg)
        {
            return true;
        }

        private void SelectedSentenceChangedCommandExecute(object obj)
        {
            if ((SelectedSentence == null) || (SentenceEditViews == null))
            {
                return;
            }

            if (SentenceEditViews.Any())
            {
                var sentenceEditView = SentenceEditViews.FirstOrDefault(
                    s =>
                        {
                            var sentenceEditorViewModel = s.DataContext as SentenceEditorViewModel;
                            return (sentenceEditorViewModel != null)
                                   && (sentenceEditorViewModel.Sentence.Id == SelectedSentence.Id);
                        });

                if (sentenceEditView != null)
                {
                    SelectedElementAttributeEditorViewModel = new ElementAttributeEditorViewModel(
                        eventAggregator, sentenceEditView.ViewId)
                    {
                        Attributes = SelectedSentence.Attributes
                    };

                    ActiveSentenceEditorView = sentenceEditView;
                }
            }

            var sentenceIdAttribute = SelectedSentence.Attributes.FirstOrDefault(a => a.Name.Equals("id"));
            var documentIdAttribute = SelectedDocument.Attributes.FirstOrDefault(a => a.Name.Equals("id"));

            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(
                    string.Format(
                        "Selected sentence with ID: {0} from document with ID: {1}", 
                        sentenceIdAttribute != null ? sentenceIdAttribute.Value : string.Empty, 
                        documentIdAttribute != null ? documentIdAttribute.Value : string.Empty));
        }

        private void EditSentenceCommandExecute(object obj)
        {
            if ((SelectedSentence == null) || (SentenceEditViews == null))
            {
                return;
            }

            var sentenceEditView =
                new SentenceEditorView(
                    new SentenceEditorViewModel(eventAggregator, appConfig, SelectedSentence, showInfoMessage), 
                    eventAggregator, 
                    appConfig);

            SentenceEditViews.Add(sentenceEditView);
            ActiveSentenceEditorView = sentenceEditView;
            SelectedElementAttributeEditorViewModel = new ElementAttributeEditorViewModel(
                eventAggregator, 
                sentenceEditView.ViewId)
            {
                Attributes = SelectedSentence.Attributes
            };

            var sentenceIdAttribute = SelectedSentence.Attributes.FirstOrDefault(a => a.Name.Equals("id"));
            var documentIdAttribute = SelectedDocument.Attributes.FirstOrDefault(a => a.Name.Equals("id"));

            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(
                    string.Format(
                        "Editing sentence with ID: {0}, document ID: {1}", 
                        sentenceIdAttribute != null ? sentenceIdAttribute.Value : string.Empty, 
                        documentIdAttribute != null ? documentIdAttribute.Value : string.Empty));
        }

        private bool EditSentenceCommandCanExecute(object arg)
        {
            if (SelectedSentence != null)
            {
                return true;
            }

            return false;
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)NewTreeBankCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)OpenCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveAsCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CloseCommand).RaiseCanExecuteChanged();

            ((DelegateCommand)AddSentenceCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteSentenceCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)EditSentenceCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)EditWordOrderCommand).RaiseCanExecuteChanged();
        }

        private bool CloseCommandCanExecute(object arg)
        {
            return SelectedDocument != null;
        }

        private void CloseCommandExecute(object obj)
        {
            if ((SelectedDocument == null) || (documentsWrappers == null))
            {
                return;
            }

            var closedDocumentFilepath = SelectedDocument.Model.FilePath;

            if (closedDocumentFilepath != null)
            {
                documentsWrappers.Remove(closedDocumentFilepath);
            }

            SelectedDocument = null;

            if (documentsWrappers.Any())
            {
                SelectedDocument = documentsWrappers.First().Value;
            }

            RefreshDocumentsExplorerList();
            InvalidateCommands();

            NotifiyDocumentClosed(closedDocumentFilepath);
        }

        private void NotifiyDocumentClosed(string closedDocumentFilepath)
        {
            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(string.Format("Document closed: {0}", closedDocumentFilepath));
        }

        private bool NewTreeBankCommandCanExecute(object arg)
        {
            return true;
        }

        private void NewTreeBankCommandExecute(object obj)
        {
            var document = new Document();

            document.Attributes.Add(
                new Attribute { Name = "id", DisplayName = "Id", Value = "Treebank" + Documents.Count });

            if (Documents == null)
            {
                Documents =
                    new ChangeTrackingCollection<DocumentWrapper>(
                        new List<DocumentWrapper> { new DocumentWrapper(document) });
            }
            else
            {
                Documents.Add(new DocumentWrapper(document));
            }

            InvalidateCommands();

            eventAggregator.GetEvent<StatusNotificationEvent>().Publish("Treebank created");
        }

        private bool OpenCommandCanExecute(object arg)
        {
            return true;
        }

        private async void OpenCommandExecute(object obj)
        {
            var documentFilePath = openFileDialogService.GetFileLocation(FileFilters.XmlFilesOnlyFilter);

            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(string.Format("Loading document: {0}. Please wait...", documentFilePath));

            if (string.IsNullOrWhiteSpace(documentFilePath))
            {
                return;
            }

            DocumentLoadExceptions.Clear();

            var documentModel =
                await documentMapper.Map(documentFilePath, ConfigurationManager.AppSettings["configurationFilePath"]);

            // todo: must check if the file is alredy loaded and has changes offer to save if so
            documentsWrappers[documentFilePath] = new DocumentWrapper(documentModel);

            RefreshDocumentsExplorerList();

            InvalidateCommands();

            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(string.Format("Document loaded: {0}", documentFilePath));
        }

        private void RefreshDocumentsExplorerList()
        {
            if (Documents == null)
            {
                return;
            }

            Documents.Clear();

            foreach (var documentWrapper in documentsWrappers)
            {
                Documents.Add(documentWrapper.Value);
            }
        }

        private void SaveCommandExecute(object obj)
        {
            var documentFilePath = SelectedDocument != null
                                       ? SelectedDocument.Model.FilePath
                                       : saveDialogService.GetSaveFileLocation(FileFilters.XmlFilesOnlyFilter);

            eventAggregator.GetEvent<StatusNotificationEvent>().Publish("Saving document");

            if (string.IsNullOrWhiteSpace(documentFilePath))
            {
            }

            // todo: save logic
            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(string.Format("Document saved: {0}", documentFilePath));
        }

        private bool SaveCommandCanExecute(object arg)
        {
            return SelectedDocument != null;
        }

        private bool SaveAsCommandCanExecute(object arg)
        {
            return SelectedDocument != null;
        }

        private void SaveAsCommandExecute(object obj)
        {
            eventAggregator.GetEvent<StatusNotificationEvent>().Publish("Saving document");

            var documentFilePath = saveDialogService.GetSaveFileLocation(FileFilters.AllFilesFilter);

            if (string.IsNullOrWhiteSpace(documentFilePath))
            {
            }

            // todo: save as logic
            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish(string.Format("Document saved: {0}", documentFilePath));
        }
    }
}