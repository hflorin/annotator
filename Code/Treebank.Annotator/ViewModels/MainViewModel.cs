namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using Events;
    using GraphX.PCL.Logic.Helpers;
    using Mappers;
    using Mappers.Algos;
    using Mappers.Configuration;
    using Mappers.LightWeight;
    using Persistence;
    using Prism.Events;
    using Treebank.Events;
    using View;
    using View.Services;
    using Wrapper;
    using Wrapper.Base;
    using Attribute = Domain.Attribute;

    public class MainViewModel : Observable
    {
        private ISentenceEditorView activeSentenceEditorView;

        private IAppConfigMapper appConfigMapper;

        private string currentStatus;

        private Dictionary<string, DocumentWrapper> documentsWrappers;

        private IEventAggregator eventAggregator;

        private IOpenFileDialogService openFileDialogService;

        private ISaveDialogService saveDialogService;

        private DocumentWrapper selectedDocument;

        private ElementAttributeEditorViewModel selectedElementAttributeEditorViewModel;

        private SentenceWrapper sentence;

        private ObservableCollection<ISentenceEditorView> sentenceEditViewModels;

        private IShowInfoMessage showInfoMessage;

        public MainViewModel(
            IEventAggregator eventAggregator,
            ISaveDialogService saveDialogService,
            IOpenFileDialogService openFileDialogService,
            IAppConfigMapper appConfigMapper,
            IShowInfoMessage showInfoMessage)
        {
            InitializeCommands();

            InitializeServices(
                eventAggregator,
                saveDialogService,
                openFileDialogService,
                appConfigMapper,
                showInfoMessage);

            if (!EnsureConfigurationsAreAvailable())
            {
                return;
            }

            SubscribeToEvents();

            InitializeMembers();
        }

        public ObservableCollection<DocumentWrapper> Documents { get; set; }

        public ObservableCollection<string> DocumentLoadExceptions { get; set; }

        public string CurrentStatus
        {
            get { return currentStatus; }

            set
            {
                currentStatus = value;
                OnPropertyChanged();
            }
        }

        public SentenceWrapper SelectedSentence
        {
            get { return sentence; }

            set
            {
                sentence = value;

                ((DelegateCommand) EditSentenceCommand).RaiseCanExecuteChanged();
                ((DelegateCommand) EditWordOrderCommand).RaiseCanExecuteChanged();
                ((DelegateCommand) AddSentenceCommand).RaiseCanExecuteChanged();
                ((DelegateCommand) DeleteSentenceCommand).RaiseCanExecuteChanged();

                OnPropertyChanged();
            }
        }

        public DocumentWrapper SelectedDocument
        {
            get { return selectedDocument; }

            set
            {
                selectedDocument = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ISentenceEditorView> SentenceEditViews
        {
            get { return sentenceEditViewModels; }

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

        public ICommand CompareSentencesCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand EditSentenceCommand { get; set; }

        public ICommand AddSentenceCommand { get; set; }

        public ICommand DeleteSentenceCommand { get; set; }

        public ICommand EditWordOrderCommand { get; set; }

        public ICommand SelectedSentenceChangedCommand { get; set; }

        public ISentenceEditorView ActiveSentenceEditorView
        {
            get { return activeSentenceEditorView; }

            set
            {
                activeSentenceEditorView = value;
                OnPropertyChanged();
            }
        }

        public ElementAttributeEditorViewModel SelectedElementAttributeEditorViewModel
        {
            get { return selectedElementAttributeEditorViewModel; }

            set
            {
                selectedElementAttributeEditorViewModel = value;
                OnPropertyChanged();
            }
        }

        public DataStructure DataStructure
        {
            get
            {
                var extension = Path.GetExtension(selectedDocument.FilePath);
                if (extension != null)
                {
                    var fileFormat = extension.Substring(1).ToLowerInvariant();

                    var appconfig =
                        appConfigMapper.Map(SelectedDocument.Model.GetAttributeByName("configurationFilePath"))
                            .GetAwaiter()
                            .GetResult();

                    return appconfig.DataStructures.FirstOrDefault(d => fileFormat == d.Format);
                }

                return null;
            }
        }

        public void OnClosing(CancelEventArgs cancelEventArgs)
        {
            var modifiedDocs =
                Documents.Where(p => p.IsChanged || string.IsNullOrWhiteSpace(p.FilePath)).Select(p => p).ToList();

            if (modifiedDocs.Any())
            {
                if (
                    showInfoMessage.ShowInfoMessage(
                        "Unsaved changes will be lost upon closing the application.\r\nDo you want to save the changes?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (var doc in modifiedDocs)
                    {
                        if (string.IsNullOrEmpty(doc.FilePath))
                        {
                            doc.FilePath = saveDialogService.GetSaveFileLocation(FileFilters.XmlFilesOnlyFilter);
                        }

                        Save(doc.Model);
                    }
                }
            }
        }

        public void Save(Document document)
        {
            var documentFilePath = SelectedDocument != null
                ? SelectedDocument.Model.FilePath
                : saveDialogService.GetSaveFileLocation(FileFilters.XmlFilesOnlyFilter);

            if (!string.IsNullOrWhiteSpace(documentFilePath))
            {
                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(string.Format("Saving document to file {0}", documentFilePath));

                Save(document, documentFilePath, true);

                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(string.Format("Document saved: {0}", documentFilePath));

                return;
            }

            eventAggregator.GetEvent<StatusNotificationEvent>()
                .Publish("Could not save file because there was no filepath set.");
        }

        // todo use abstract factory and inject the factory to encapsulate the persisting logic better
        public void Save(Document document, string documentFilePath, bool overwrite)
        {
            PersisterClient persister = null;

            var extension = Path.GetExtension(documentFilePath);
            if (extension != null)
            {
                var lowercaseExtension = extension.Substring(1).ToLowerInvariant();

                if (lowercaseExtension.Equals(ConfigurationStaticData.XmlFormat))
                {
                    persister = new PersisterClient(new XmlPersister(eventAggregator));
                }
                else if (lowercaseExtension.Equals(ConfigurationStaticData.ConllxFormat)
                         || lowercaseExtension.Equals(ConfigurationStaticData.ConllFormat))
                {
                    persister = new PersisterClient(new ConllxPersister(eventAggregator));
                }
            }

            if (persister != null)
            {
                persister.Save(document, documentFilePath, overwrite);
            }
            else
            {
                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        "Cannot save the document selected,because the format is not supported. Supported formats are XML and CONLLX.");
            }
        }

        private bool EnsureConfigurationsAreAvailable()
        {
            var configFilesDirectoryPath = ConfigurationManager.AppSettings["configurationFilesDirectoryPath"];

            var filesCount = 0;

            if (!string.IsNullOrWhiteSpace(configFilesDirectoryPath) && Directory.Exists(configFilesDirectoryPath))
            {
                filesCount = Directory.GetFiles(configFilesDirectoryPath, "*.xml").Length;
            }

            while (string.IsNullOrWhiteSpace(configFilesDirectoryPath) || !Directory.Exists(configFilesDirectoryPath)
                   || (filesCount == 0))
            {
                if (
                    showInfoMessage.ShowInfoMessage(
                        "The folder path to the configuration files is not set.\r\nPlease choose the location where the configuration files are located.",
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    configFilesDirectoryPath = openFileDialogService.GetFolderLocation();
                }
                else
                {
                    eventAggregator.GetEvent<CloseApplicationEvent>().Publish(true);
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(configFilesDirectoryPath))
                {
                    filesCount = Directory.GetFiles(configFilesDirectoryPath, "*.xml").Length;
                }
            }

            SetPathInAppSettings(configFilesDirectoryPath);

            return true;
        }

        private void InitializeMembers()
        {
            documentsWrappers = new Dictionary<string, DocumentWrapper>();
            DocumentLoadExceptions = new ObservableCollection<string>();
            Documents = new ObservableCollection<DocumentWrapper>();
            sentenceEditViewModels = new ObservableCollection<ISentenceEditorView>();
        }

        private void SubscribeToEvents()
        {
            eventAggregator.GetEvent<ValidationExceptionEvent>().Subscribe(OnDocumentLoadException);
            eventAggregator.GetEvent<StatusNotificationEvent>().Subscribe(OnStatusNotification);
            eventAggregator.GetEvent<ChangeAttributesEditorViewModel>().Subscribe(OnAttributesChanged);
            eventAggregator.GetEvent<CheckIsTreeOnSentenceEvent>().Subscribe(OnCheckIsTreeOnSentence);
            eventAggregator.GetEvent<UpdateAllViewsForSentenceByViewId>().Subscribe(OnUpdateAllViewsForSentenceByViewId);
        }

        private void OnUpdateAllViewsForSentenceByViewId(Guid viewId)
        {
            if ((sentenceEditViewModels == null) || !sentenceEditViewModels.Any())
            {
                return;
            }

            var sentenceViewToUpdate =
                sentenceEditViewModels.FirstOrDefault(sev => sev is SentenceEditorView && (sev.ViewId == viewId)) as
                    SentenceEditorView;

            if (sentenceViewToUpdate != null)
            {
                var sentenceId = sentenceViewToUpdate.ViewModel.Sentence.Id.Value;
                var sentenceEditViewsToUpdate =
                    sentenceEditViewModels.Where(
                        sev =>
                            sev is SentenceEditorView
                            && (((SentenceEditorView) sev).ViewModel.Sentence.Id.Value == sentenceId)).ToList();

                if (sentenceEditViewsToUpdate.Any())
                {
                    sentenceEditViewsToUpdate.ForEach(
                        v => eventAggregator.GetEvent<GenerateGraphEvent>().Publish(v.ViewId));
                }
            }
        }

        private void OnCheckIsTreeOnSentence(SentenceWrapper sentenceWrapper)
        {
            if (sentenceWrapper == null)
            {
                return;
            }

            var validationResult = new CheckGraphResult();

            // todo no bueno with the app config
            var appConfig =
                appConfigMapper.Map(SelectedDocument.Model.GetAttributeByName("configurationFilePath"))
                    .GetAwaiter()
                    .GetResult();

            sentenceWrapper.IsTree =
                GraphOperations.GetGraph(sentenceWrapper.Model, appConfig.Definitions.First(), eventAggregator)
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
            appConfigMapper = configMapper;
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
            CompareSentencesCommand = new DelegateCommand(
                CompareSentencesCommandExecute,
                CompareSentencesCommandCanExecute);
        }

        private void CompareSentencesCommandExecute(object obj)
        {
            var compareSentencesWindow =
                new CompareSentencesWindow(new CompareSentencesViewModel(eventAggregator, Documents));

            if (!compareSentencesWindow.ShowDialog().GetValueOrDefault())
            {
                return;
            }

            var dataContext = compareSentencesWindow.DataContext as CompareSentencesViewModel;
            if (dataContext != null)
            {
                if ((dataContext.LeftSelectedDocument == null) || (dataContext.RightSelectedDocument == null)
                    || (SentenceEditViews == null))
                {
                    return;
                }

                var leftSentenceIdAttribute =
                    dataContext.LeftSelectedSentence.Attributes.FirstOrDefault(a => a.Name.Equals("id"));
                var leftDocumentIdAttribute =
                    dataContext.LeftSelectedDocument.Attributes.FirstOrDefault(a => a.Name.Equals("id"));

                var rightSentenceIdAttribute =
                    dataContext.RightSelectedSentence.Attributes.FirstOrDefault(a => a.Name.Equals("id"));
                var rightDocumentIdAttribute =
                    dataContext.RightSelectedDocument.Attributes.FirstOrDefault(a => a.Name.Equals("id"));

                var leftSentenceIdAttributeValue = leftSentenceIdAttribute == null
                    ? string.Empty
                    : leftSentenceIdAttribute.Value;
                var leftDocumentIdAttributeValue = leftDocumentIdAttribute == null
                    ? string.Empty
                    : leftDocumentIdAttribute.Value;

                var rightSentenceIdAttributeValue = rightSentenceIdAttribute == null
                    ? string.Empty
                    : rightSentenceIdAttribute.Value;
                var rightDocumentIdAttributeValue = rightDocumentIdAttribute == null
                    ? string.Empty
                    : rightDocumentIdAttribute.Value;

                var appConfig =
                    appConfigMapper.Map(SelectedDocument.Model.GetAttributeByName("configurationFilePath"))
                        .GetAwaiter()
                        .GetResult();

                var sentenceEditView =
                    new CompareSentenceEditorView(
                        new SentenceEditorViewModel(
                            eventAggregator,
                            appConfig,
                            dataContext.LeftSelectedSentence,
                            dataContext.RightSelectedSentence,
                            showInfoMessage)
                        {
                            LeftSentenceInfo =
                                new StringWrapper(
                                    string.Format(
                                        "Sentence id {0}, Document Id {1}",
                                        leftSentenceIdAttributeValue,
                                        leftDocumentIdAttributeValue)),
                            RightSentenceInfo =
                                new StringWrapper(
                                    string.Format(
                                        "Sentence id {0}, Document Id {1}",
                                        rightSentenceIdAttributeValue,
                                        rightDocumentIdAttributeValue))
                        },
                        eventAggregator,
                        appConfig);

                SentenceEditViews.Add(sentenceEditView);
                ActiveSentenceEditorView = sentenceEditView;

                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(
                        string.Format(
                            "Comparing sentences: {0} (document {1}) and {2} (document {3}) ",
                            leftSentenceIdAttributeValue,
                            leftDocumentIdAttributeValue,
                            rightSentenceIdAttributeValue,
                            rightDocumentIdAttributeValue));
            }

            InvalidateCommands();
        }

        private bool CompareSentencesCommandCanExecute(object arg)
        {
            return Documents.Any();
        }

        private void BindAttributesCommandExecute(object obj)
        {
            if (SelectedDocument != null)
            {
                eventAggregator.GetEvent<ChangeAttributesEditorViewModel>()
                    .Publish(
                        new ElementAttributeEditorViewModel(eventAggregator, Guid.Empty)
                        {
                            Attributes =
                                SelectedDocument
                                    .Attributes
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
                    SelectedDocument.Sentences.Except(new List<SentenceWrapper> {SelectedSentence}).FirstOrDefault();
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
                var appconfig =
                    appConfigMapper.Map(SelectedDocument.Model.GetAttributeByName("configurationFilePath"))
                        .GetAwaiter()
                        .GetResult();

                var sentencePrototype = DataStructure.Elements.OfType<Sentence>().FirstOrDefault();
                var wordPrototype = DataStructure.Elements.OfType<Word>().FirstOrDefault();
                var configuration = appconfig.Definitions.FirstOrDefault();

                if (configuration == null)
                {
                    eventAggregator.GetEvent<StatusNotificationEvent>()
                        .Publish("Must define a configuration in the configuration file before adding a sentence");
                    return;
                }

                var inputDialog = new InputDialog("Enter sentence");

                if (inputDialog.ShowDialog().GetValueOrDefault())
                {
                    var sentenceContent = inputDialog.Value;

                    if (sentencePrototype != null)
                    {
                        var sentenceClone = ObjectCopier.Clone(sentencePrototype);

                        sentenceClone.SetAttributeByName("date", DateTime.Now.ToString("dd-MM-yyyy"));
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

                        var words = sentenceContent.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                        for (var i = 0; i < words.Length; i++)
                        {
                            var wordContent = words[i];
                            var newWord = ObjectCopier.Clone(wordPrototype);
                            newWord.Value = wordContent;

                            newWord.SetAttributeByName(configuration.Edge.TargetVertexAttributeName, (i + 1).ToString());
                            newWord.SetAttributeByName(configuration.Edge.SourceVertexAttributeName, "0");

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
                eventAggregator.GetEvent<UpdateAllViewsForSentenceByViewId>().Publish(ActiveSentenceEditorView.ViewId);
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
                        var view = s as SentenceEditorView;
                        if (view != null)
                        {
                            var sentenceEditorViewModel = view.DataContext as SentenceEditorViewModel;
                            return (sentenceEditorViewModel != null)
                                   && (sentenceEditorViewModel.Sentence.Id == SelectedSentence.Id);
                        }

                        return false;
                    });

                var sentenceEditViewId = Guid.Empty;

                if (sentenceEditView != null)
                {
                    sentenceEditViewId = sentenceEditView.ViewId;

                    ActiveSentenceEditorView = sentenceEditView;
                }

                SelectedElementAttributeEditorViewModel = new ElementAttributeEditorViewModel(
                    eventAggregator, sentenceEditViewId)
                {
                    Attributes = SelectedSentence.Attributes
                };
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

            var appConfig =
                appConfigMapper.Map(SelectedDocument.Model.GetAttributeByName("configurationFilePath"))
                    .GetAwaiter()
                    .GetResult();

            if ((SelectedSentence.Words == null) || !SelectedSentence.Words.Any())
            {
                LoadWordsForSentence(SelectedSentence, SelectedDocument.FilePath, appConfig.Filepath)
                    .GetAwaiter()
                    .GetResult();
            }

            var sentenceEditView =
                new SentenceEditorView(
                    new SentenceEditorViewModel(
                        eventAggregator,
                        appConfig,
                        DataStructure,
                        SelectedSentence,
                        showInfoMessage),
                    eventAggregator);

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

        private async Task LoadWordsForSentence(
            SentenceWrapper selectedSentence,
            string documentFilePath,
            string configFilePath)
        {
            var extension = Path.GetExtension(documentFilePath);
            if (extension != null)
            {
                var lowercaseExtension = extension.Substring(1).ToLowerInvariant();

                DocumentMapperClient documentMapper = null;

                if (lowercaseExtension.Equals(ConfigurationStaticData.XmlFormat))
                {
                    documentMapper =
                        new DocumentMapperClient(
                            new LightDocumentMapperWithReader
                            {
                                AppConfigMapper = appConfigMapper,
                                EventAggregator = eventAggregator
                            });
                }
                else if (lowercaseExtension.Equals(ConfigurationStaticData.ConllxFormat)
                         || lowercaseExtension.Equals(ConfigurationStaticData.ConllFormat))
                {
                    documentMapper =
                        new DocumentMapperClient(
                            new LightConllxDocumentMapper
                            {
                                AppConfigMapper = appConfigMapper,
                                EventAggregator = eventAggregator
                            });
                }

                if (documentMapper == null)
                {
                    return;
                }

                var sentenceLoaded =
                    await documentMapper.LoadSentence(selectedSentence.Id.Value, documentFilePath, configFilePath);

                if (sentenceLoaded == null)
                {
                    return;
                }

                var selectedSentenceIndex = SelectedDocument.Sentences.IndexOf(SelectedSentence);

                SelectedDocument.Sentences[selectedSentenceIndex] = new SentenceWrapper(sentenceLoaded);

                SelectedSentence = SelectedDocument.Sentences[selectedSentenceIndex];
            }

            SelectedDocument.AcceptChanges();
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
            ((DelegateCommand) NewTreeBankCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) OpenCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) SaveAsCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) CloseCommand).RaiseCanExecuteChanged();

            ((DelegateCommand) AddSentenceCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) DeleteSentenceCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) EditSentenceCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) EditWordOrderCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) CompareSentencesCommand).RaiseCanExecuteChanged();
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

            if (selectedDocument.IsChanged)
            {
                if (
                    showInfoMessage.ShowInfoMessage(
                        "Unsaved changes will be lost upon closing the document.\r\nDo you want to save the changes?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(selectedDocument.FilePath))
                    {
                        selectedDocument.FilePath = saveDialogService.GetSaveFileLocation(
                            FileFilters.XmlFilesOnlyFilter);
                    }

                    Save(selectedDocument.Model);
                }
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
            if (EnsureConfigurationsAreAvailable())
            {
                IAppConfig appConfig;

                var chooseConfigWindow = new ChooseConfigurationWindow(appConfigMapper);

                if (chooseConfigWindow.ShowDialog().GetValueOrDefault())
                {
                    var configFilePath = chooseConfigWindow.SelectedConfigFile.FilePath;
                    appConfig = appConfigMapper.Map(configFilePath).GetAwaiter().GetResult();
                }
                else
                {
                    return;
                }

                var documentPrototype = DataStructure.Elements.OfType<Document>().Single();

                var document = ObjectCopier.Clone(documentPrototype);

                document.SetAttributeByName("id", "Treebank" + Documents.Count);

                var filenameToPathMapping = AppConfig.GetConfigFileNameToFilePathMapping();

                document.Attributes.Add(
                    new Attribute
                    {
                        AllowedValuesSet = filenameToPathMapping.Values,
                        Value = appConfig.Name,
                        Name = "configuration",
                        DisplayName = "Configuration",
                        Entity = "attribute",
                        IsEditable = true,
                        IsOptional = false
                    });

                if (Documents == null)
                {
                    Documents =
                        new ChangeTrackingCollection<DocumentWrapper>(
                            new List<DocumentWrapper> {new DocumentWrapper(document)});
                }
                else
                {
                    Documents.Add(new DocumentWrapper(document));
                }

                InvalidateCommands();

                eventAggregator.GetEvent<StatusNotificationEvent>().Publish("Treebank created");
            }
        }

        private bool OpenCommandCanExecute(object arg)
        {
            return true;
        }

        private async void OpenCommandExecute(object obj)
        {
            if (EnsureConfigurationsAreAvailable())
            {
                IAppConfig appConfig;

                var chooseConfigWindow = new ChooseConfigurationWindow(appConfigMapper);

                if (chooseConfigWindow.ShowDialog().GetValueOrDefault())
                {
                    var configFilePath = chooseConfigWindow.SelectedConfigFile.FilePath;
                    appConfig = await appConfigMapper.Map(configFilePath);
                }
                else
                {
                    return;
                }

                var documentFilePath = openFileDialogService.GetFileLocation(FileFilters.XmlAndConllxFilesOnlyFilter);

                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(string.Format("Loading document: {0}. Please wait...", documentFilePath));

                if (string.IsNullOrWhiteSpace(documentFilePath))
                {
                    return;
                }

                DocumentLoadExceptions.Clear();

                var documentModel = await MapDocumentModel(documentFilePath, appConfig);

                if (documentModel == null)
                {
                    return;
                }

                var documentWrapper = new DocumentWrapper(documentModel);

                documentsWrappers[documentFilePath] = documentWrapper;

                RefreshDocumentsExplorerList();

                InvalidateCommands();

                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(string.Format("Document loaded: {0}", documentFilePath));
            }
        }

        private async Task<Document> MapDocumentModel(string documentFilePath, IAppConfig appConfig)
        {
            Document documentModel = null;

            var extension = Path.GetExtension(documentFilePath);
            if (extension != null)
            {
                var lowercaseExtension = extension.Substring(1).ToLowerInvariant();

                if (lowercaseExtension.Equals(ConfigurationStaticData.XmlFormat))
                {
                    documentModel =
                        await
                            new DocumentMapperClient(
                                new LightDocumentMapperWithReader
                                {
                                    AppConfigMapper = appConfigMapper,
                                    EventAggregator = eventAggregator
                                }).Map(
                                    documentFilePath,
                                    appConfig.Filepath);
                }
                else if (lowercaseExtension.Equals(ConfigurationStaticData.ConllxFormat)
                         || lowercaseExtension.Equals(ConfigurationStaticData.ConllFormat))
                {
                    documentModel =
                        await
                            new DocumentMapperClient(
                                new LightConllxDocumentMapper
                                {
                                    AppConfigMapper = appConfigMapper,
                                    EventAggregator = eventAggregator
                                }).Map(
                                    documentFilePath,
                                    appConfig.Filepath);
                }
                else
                {
                    eventAggregator.GetEvent<StatusNotificationEvent>()
                        .Publish(
                            "Cannot load the file selected,because the format is not supported. Supported formats are XML and CONLLX.");
                }
            }

            return documentModel;
        }

        private void SetPathInAppSettings(string configFilesDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(configFilesDirectoryPath))
            {
                return;
            }

            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (appPath == null)
            {
                return;
            }

            var configFile = Path.Combine(appPath, Assembly.GetExecutingAssembly().GetName().Name + ".exe.config");
            var configFileMap = new ExeConfigurationFileMap {ExeConfigFilename = configFile};
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            config.AppSettings.Settings["configurationFilesDirectoryPath"].Value = configFilesDirectoryPath;
            config.Save();
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
            Save(SelectedDocument.Model);
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

            var documentFilePath = saveDialogService.GetSaveFileLocation(FileFilters.XmlAndConllxFilesOnlyFilter);

            if (!string.IsNullOrWhiteSpace(documentFilePath))
            {
                if (SelectedDocument != null)
                {
                    SelectedDocument.Model.FilePath = documentFilePath;
                    Save(SelectedDocument.Model, documentFilePath, false);
                }

                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(string.Format("Document saved: {0}", documentFilePath));
            }
        }
    }
}