namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using Graph;
    using Graph.Algos;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
    using Mappers;
    using Mappers.Configuration;
    using Prism.Events;
    using Treebank.Events;
    using View;
    using View.Services;
    using Wrapper;
    using Wrapper.Base;

    public class SentenceEditorViewModel : Observable
    {
        private readonly IAppConfig appConfig;

        private readonly GraphBuilder graphBuilder;

        private readonly IShowInfoMessage showMessage;

        private ObservableCollection<GraphLayoutAlgorithmTypeEnum> layoutAlgorithmTypes = new ObservableCollection
            <GraphLayoutAlgorithmTypeEnum>(
            Enum.GetValues(typeof(GraphLayoutAlgorithmTypeEnum)).Cast<GraphLayoutAlgorithmTypeEnum>());

        private SenteceGraphOperationMode operationMode = SenteceGraphOperationMode.Select;

        private SentenceGxLogicCore sentenceLogicCore;

        private SentenceWrapper sentenceWrapper;

        public SentenceEditorViewModel(
            IEventAggregator eventAggregator,
            IAppConfig appConfig,
            SentenceWrapper sentence,
            IShowInfoMessage showMessage)
        {
            if (sentence == null)
            {
                throw new ArgumentNullException("sentence");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            if (appConfig == null)
            {
                throw new ArgumentNullException("appConfig");
            }

            if (showMessage == null)
            {
                throw new ArgumentNullException("showMessage");
            }

            InitializeCommands();

            EventAggregator = eventAggregator;
            Sentence = sentence;
            graphBuilder = new GraphBuilder(appConfig, appConfig.Definitions.First());
            this.appConfig = appConfig;
            this.showMessage = showMessage;
            GraphConfigurations = new ObservableCollection<Definition>(this.appConfig.Definitions);
            SelectedGraphConfiguration = GraphConfigurations.First();

            PopulateWords(eventAggregator, sentence);

            var sentenceGraph = new SentenceGraph();
            sentenceLogicCore = new SentenceGxLogicCore {Graph = sentenceGraph};
        }

        public SenteceGraphOperationMode SenteceGraphOperationMode
        {
            get { return operationMode; }

            set { operationMode = value; }
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICommand CheckIsTreeCommand { get; set; }

        public ICommand LayoutAlgorithmChangedCommand { get; set; }

        public ICommand AddWordCommand { get; set; }

        public ICommand GraphConfigurationChangedCommand { get; set; }

        public ICommand ToggleEditModeCommand { get; set; }

        public SentenceWrapper Sentence
        {
            get { return sentenceWrapper; }

            set
            {
                sentenceWrapper = value;
                OnPropertyChanged();
            }
        }

        public ChangeTrackingCollection<WordEditorViewModel> Words { get; set; }

        public SentenceGxLogicCore SentenceGraphLogicCore
        {
            get { return sentenceLogicCore; }

            set
            {
                sentenceLogicCore = value;
                OnPropertyChanged();
            }
        }

        public GraphLayoutAlgorithmTypeEnum SelectedLayoutAlgorithmType { get; set; }

        public Definition SelectedGraphConfiguration { get; set; }

        public ObservableCollection<GraphLayoutAlgorithmTypeEnum> LayoutAlgorithmTypes
        {
            get { return layoutAlgorithmTypes; }

            set
            {
                layoutAlgorithmTypes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Definition> GraphConfigurations { get; set; }

        public void PopulateWords()
        {
            PopulateWords(EventAggregator, sentenceWrapper);
        }

        private void PopulateWords(IEventAggregator eventAggregator, SentenceWrapper sentence)
        {
            var sortedWords = sentence.Words.ToList();
            sortedWords.Sort(Comparison);

            IList<WordEditorViewModel> w = new List<WordEditorViewModel>();

            foreach (var word in sortedWords)
            {
                w.Add(new WordEditorViewModel(word, eventAggregator));
            }

            Words = new ChangeTrackingCollection<WordEditorViewModel>(w);
            OnPropertyChanged("Words");
        }

        private int Comparison(WordWrapper left, WordWrapper right)
        {
            var leftId = int.Parse(left.GetAttributeByName("id"));
            var rightId = int.Parse(right.GetAttributeByName("id"));

            if (leftId == rightId)
            {
                return 0;
            }

            return leftId > rightId ? 1 : -1;
        }

        private void InitializeCommands()
        {
            LayoutAlgorithmChangedCommand = new DelegateCommand(
                LayoutAlgorithmChangedCommandExecute,
                LayoutAlgorithmChangedCommandCanExecute);
            GraphConfigurationChangedCommand = new DelegateCommand(
                GraphConfigurationChangedCommandExecute,
                GraphConfigurationChangedCommandCanExecute);
            ToggleEditModeCommand = new DelegateCommand(ToggleEditModeCommandExecute, ToggleEditModeCommandCanExecute);
            AddWordCommand = new DelegateCommand(AddWordCommandExecute, AddWordCommandCanExecute);
            CheckIsTreeCommand = new DelegateCommand(CheckIsTreeCommandExecute, CheckIsTreeCommandCanExecute);
        }

        private void CheckIsTreeCommandExecute(object obj)
        {
            var validationResult = new CheckGraphResult();
            var isTree =
                GraphOperations.GetGraph(Sentence, appConfig.Definitions.First(), EventAggregator)
                    .IsTree(validationResult);

            if (!isTree)
            {
                foreach (var disconnectedWordId in validationResult.DisconnectedWordIds)
                {
                    EventAggregator.GetEvent<ValidationExceptionEvent>()
                        .Publish(string.Format("The word with id: {0}, is not connected to another word.",
                            disconnectedWordId));
                }

                foreach (var cycle in validationResult.Cycles)
                {
                    EventAggregator.GetEvent<ValidationExceptionEvent>()
                        .Publish(string.Format("The sentence with id {0} has cycle: {1}",
                            Sentence.Id.Value, string.Join(",", cycle)));
                }

                if (validationResult.DisconnectedWordIds.Any() || validationResult.Cycles.Any())
                {
                    EventAggregator.GetEvent<StatusNotificationEvent>()
                        .Publish("Please check warnings in the Output panel.");
                }
            }

            showMessage.ShowInfoMessage(
                string.Format("Graph for sentence with id {0} is tree: {1}", Sentence.Id.Value, isTree));
        }

        private bool CheckIsTreeCommandCanExecute(object arg)
        {
            return true;
        }

        private void AddWordCommandExecute(object obj)
        {
            var wordPrototype = ObjectCopier.Clone(appConfig.Elements.OfType<Word>().Single());
            var wordIds =
                Sentence.Words.Select(
                    w => new Pair {Id = int.Parse(w.GetAttributeByName("id")), Form = w.GetAttributeByName("form")})
                    .ToList();
            var addWordWindow = new AddWordWindow(new AddWordViewModel(wordPrototype, wordIds));

            if (addWordWindow.ShowDialog().GetValueOrDefault())
            {
                var word = ((AddWordViewModel) addWordWindow.DataContext).Word;
                Sentence.Words.Add(word);

                var wordReorderingWindow = new WordReorderingWindow(new WordReorderingViewModel(Sentence));
                if (wordReorderingWindow.ShowDialog().GetValueOrDefault())
                {
                }
                //todo: remove if not used anymore or bring it back if performance becomes an issue
                //EventAggregator.GetEvent<AddWordVertexEvent>().Publish(word);
            }
            CreateSentenceGraph();
            PopulateWords(EventAggregator, Sentence);
        }

        private bool AddWordCommandCanExecute(object arg)
        {
            return true;
        }

        private bool ToggleEditModeCommandCanExecute(object arg)
        {
            return true;
        }

        private void ToggleEditModeCommandExecute(object obj)
        {
            EventAggregator.GetEvent<SetSentenceEditModeEvent>()
                .Publish(
                    new SetSenteceGraphOperationModeRequest
                    {
                        Mode =
                            obj is SenteceGraphOperationMode
                                ? (SenteceGraphOperationMode) obj
                                : SenteceGraphOperationMode.Select
                    });
        }

        private bool GraphConfigurationChangedCommandCanExecute(object arg)
        {
            return true;
        }

        private void GraphConfigurationChangedCommandExecute(object obj)
        {
            CreateSentenceGraph();
            EventAggregator.GetEvent<GenerateGraphEvent>().Publish(true);
        }

        private void LayoutAlgorithmChangedCommandExecute(object obj)
        {
            SetLayoutAlgorithm(SentenceGraphLogicCore);
        }

        private bool LayoutAlgorithmChangedCommandCanExecute(object arg)
        {
            return true;
        }

        public void CreateSentenceGraph()
        {
            graphBuilder.CurrentDefinition = SelectedGraphConfiguration;
            var logicCore = graphBuilder.SetupGraphLogic(Sentence);
            SentenceGraphLogicCore = logicCore;

          //  SetLayoutAlgorithm(logicCore);
        }

        public void SetLayoutAlgorithm(SentenceGxLogicCore logicCore)
        {
            switch (SelectedLayoutAlgorithmType)
            {
                case GraphLayoutAlgorithmTypeEnum.Liniar :
                    //logicCore.ExternalEdgeRoutingAlgorithm =
                    //    new LiniarEdgeRoutingAlgorithm<WordVertex, WordEdge, SentenceGraph>(
                    //        logicCore.Graph as SentenceGraph);

                    logicCore.ExternalLayoutAlgorithm =
                        new LiniarLayoutAlgorithm<WordVertex, WordEdge, SentenceGraph>(
                            logicCore.Graph as SentenceGraph,
                            50);

                    break;
                case GraphLayoutAlgorithmTypeEnum.DiagonalLiniar :
                    //logicCore.ExternalEdgeRoutingAlgorithm =
                    //    new LiniarEdgeRoutingAlgorithm<WordVertex, WordEdge, SentenceGraph>(
                    //        logicCore.Graph as SentenceGraph);

                    logicCore.ExternalLayoutAlgorithm =
                        new LiniarLayoutAlgorithm<WordVertex, WordEdge, SentenceGraph>(
                            logicCore.Graph as SentenceGraph,
                            50, 25);
                    break;
                case GraphLayoutAlgorithmTypeEnum.EfficientSugiyama :
                    SetEfficientSugiyamaLayout(logicCore);
                    break;
                case GraphLayoutAlgorithmTypeEnum.TopBottomTree :
                    SetTreeLayout(logicCore, LayoutDirection.TopToBottom);
                    break;
                case GraphLayoutAlgorithmTypeEnum.BottomTopTree :
                    SetTreeLayout(logicCore, LayoutDirection.BottomToTop);
                    break;
                case GraphLayoutAlgorithmTypeEnum.LeftRightTree :
                    SetTreeLayout(logicCore, LayoutDirection.LeftToRight);
                    break;
                case GraphLayoutAlgorithmTypeEnum.RightLeftTree :
                    SetTreeLayout(logicCore, LayoutDirection.RightToLeft);
                    break;
                default :
                    throw new ArgumentOutOfRangeException();
            }

            EventAggregator.GetEvent<RelayoutGraphEvent>().Publish(true);
        }

        private void SetTreeLayout(SentenceGxLogicCore logicCore, LayoutDirection direction)
        {
            logicCore.ExternalEdgeRoutingAlgorithm = null;
            logicCore.ExternalLayoutAlgorithm = null;

            var parameters =
                logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.Tree) as
                    SimpleTreeLayoutParameters;

            if (parameters != null)
            {
                parameters.Direction = direction;
                parameters.LayerGap = 150;
                parameters.SpanningTreeGeneration = SpanningTreeGeneration.BFS;
                parameters.VertexGap = 75;
                logicCore.EdgeCurvingEnabled = false;
                logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Tree;
                logicCore.DefaultLayoutAlgorithmParams = parameters;
            }
        }

        private void SetEfficientSugiyamaLayout(SentenceGxLogicCore logicCore)
        {
            logicCore.ExternalEdgeRoutingAlgorithm = null;
            logicCore.ExternalLayoutAlgorithm = null;

            var layoutParameters =
                logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.EfficientSugiyama) as
                    EfficientSugiyamaLayoutParameters;

            if (layoutParameters != null)
            {
                layoutParameters.EdgeRouting = SugiyamaEdgeRoutings.Traditional;
                layoutParameters.LayerDistance = layoutParameters.VertexDistance = 75;
                logicCore.EdgeCurvingEnabled = false;
                logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama;
                logicCore.DefaultLayoutAlgorithmParams = layoutParameters;
            }
        }
    }
}