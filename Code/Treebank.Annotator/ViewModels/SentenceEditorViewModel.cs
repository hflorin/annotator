namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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

        private IEnumerable edgeRoutingAlgorithmTypes =
            Enum.GetValues(typeof(EdgeRoutingAlgorithmTypeEnum)).Cast<EdgeRoutingAlgorithmTypeEnum>();

        private IEnumerable layoutAlgorithmTypes =
            Enum.GetValues(typeof(LayoutAlgorithmTypeEnum)).Cast<LayoutAlgorithmTypeEnum>();

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

            PopulateWords(eventAggregator, sentence);

            var sentenceGraph = new SentenceGraph();
            sentenceLogicCore = new SentenceGxLogicCore();
            sentenceLogicCore.Graph = sentenceGraph;
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

        public ICommand EdgeRoutingAlgorithmChangedCommand { get; set; }

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

        public LayoutAlgorithmTypeEnum SelectedLayoutAlgorithmType { get; set; }

        public EdgeRoutingAlgorithmTypeEnum SelectedEdgeRoutingAlgorithmType { get; set; }

        public IEnumerable LayoutAlgorithmTypes
        {
            get { return layoutAlgorithmTypes; }

            set { layoutAlgorithmTypes = value; }
        }

        public IEnumerable EdgeRoutingAlgorithmTypes
        {
            get { return edgeRoutingAlgorithmTypes; }

            set { edgeRoutingAlgorithmTypes = value; }
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
            EdgeRoutingAlgorithmChangedCommand = new DelegateCommand(
                EdgeRoutingAlgorithmChangedCommandExecute,
                EdgeRoutingAlgorithmChangedCommandCanExecute);
            ToggleEditModeCommand = new DelegateCommand(ToggleEditModeCommandExecute, ToggleEditModeCommandCanExecute);
            AddWordCommand = new DelegateCommand(AddWordCommandExecute, AddWordCommandCanExecute);
            CheckIsTreeCommand = new DelegateCommand(CheckIsTreeCommandExecute, CheckIsTreeCommandCanExecute);
        }

        private void CheckIsTreeCommandExecute(object obj)
        {
            var isTree = GraphOperations.GetGraph(Sentence, appConfig.Definitions.First()).IsTree();
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

        private bool EdgeRoutingAlgorithmChangedCommandCanExecute(object arg)
        {
            return true;
        }

        private void EdgeRoutingAlgorithmChangedCommandExecute(object obj)
        {
            var newEdgeRoutingAlgorithmType = SelectedEdgeRoutingAlgorithmType;
            SentenceGraphLogicCore.DefaultEdgeRoutingAlgorithm = newEdgeRoutingAlgorithmType;
        }

        private void LayoutAlgorithmChangedCommandExecute(object obj)
        {
            var newLayoutAlgorithmType = SelectedLayoutAlgorithmType;
            SentenceGraphLogicCore.DefaultLayoutAlgorithm = newLayoutAlgorithmType;

            if (newLayoutAlgorithmType == LayoutAlgorithmTypeEnum.EfficientSugiyama)
            {
                var parameters =
                    SentenceGraphLogicCore.AlgorithmFactory.CreateLayoutParameters(
                        LayoutAlgorithmTypeEnum.EfficientSugiyama) as EfficientSugiyamaLayoutParameters;

                if (parameters != null)
                {
                    parameters.EdgeRouting = SugiyamaEdgeRoutings.Orthogonal;
                    parameters.LayerDistance = parameters.VertexDistance = 50;
                    SentenceGraphLogicCore.EdgeCurvingEnabled = false;
                    SentenceGraphLogicCore.DefaultLayoutAlgorithmParams = parameters;
                }

                SelectedEdgeRoutingAlgorithmType = EdgeRoutingAlgorithmTypeEnum.None;
            }
            else
            {
                SentenceGraphLogicCore.EdgeCurvingEnabled = true;
            }

            if (newLayoutAlgorithmType == LayoutAlgorithmTypeEnum.BoundedFR)
            {
                SentenceGraphLogicCore.DefaultLayoutAlgorithmParams =
                    SentenceGraphLogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.BoundedFR);
            }

            if (newLayoutAlgorithmType == LayoutAlgorithmTypeEnum.FR)
            {
                SentenceGraphLogicCore.DefaultLayoutAlgorithmParams =
                    SentenceGraphLogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.FR);
            }
        }

        private bool LayoutAlgorithmChangedCommandCanExecute(object arg)
        {
            return true;
        }

        public void CreateSentenceGraph()
        {
            var logicCore = graphBuilder.SetupGraphLogic(Sentence);

            SentenceGraphLogicCore = logicCore;

            SelectedLayoutAlgorithmType = LayoutAlgorithmTypeEnum.EfficientSugiyama;
            SelectedEdgeRoutingAlgorithmType = EdgeRoutingAlgorithmTypeEnum.None;
        }
    }
}