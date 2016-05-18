namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using Domain.Configuration;
    using Events;
    using Graph;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
    using Mappers;
    using Prism.Events;
    using View;
    using Wrapper;

    public class SentenceEditorViewModel : Observable
    {
        private readonly IAppConfig appConfig;

        private IEnumerable edgeRoutingAlgorithmTypes =
            Enum.GetValues(typeof(EdgeRoutingAlgorithmTypeEnum)).Cast<EdgeRoutingAlgorithmTypeEnum>();

        private IEnumerable layoutAlgorithmTypes =
            Enum.GetValues(typeof(LayoutAlgorithmTypeEnum)).Cast<LayoutAlgorithmTypeEnum>();

        private SenteceGraphOperationMode operationMode = SenteceGraphOperationMode.Select;

        private SentenceGraph sentenceGraph;

        private SentenceGxLogicCore sentenceLogicCore;

        private SentenceWrapper sentenceWrapper;

        public SentenceEditorViewModel(IEventAggregator eventAggregator, IAppConfig appConfig, SentenceWrapper sentence)
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

            InitializeCommands();

            EventAggregator = eventAggregator;
            Sentence = sentence;
            this.appConfig = appConfig;

            PopulateWords(eventAggregator, sentence);

            sentenceGraph = new SentenceGraph();
            sentenceLogicCore = new SentenceGxLogicCore();
            sentenceLogicCore.Graph = sentenceGraph;
        }

        public SenteceGraphOperationMode SenteceGraphOperationMode
        {
            get { return operationMode; }
            set { operationMode = value; }
        }

        public IEventAggregator EventAggregator { get; set; }

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

        public ObservableCollection<WordEditorViewModel> Words { get; set; }

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
            Words = new ObservableCollection<WordEditorViewModel>();

            var sortedWords = sentence.Words.ToList();
            sortedWords.Sort(Comparison);

            foreach (var word in sortedWords)
            {
                Words.Add(new WordEditorViewModel(word, eventAggregator));
            }
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
        }

        private void AddWordCommandExecute(object obj)
        {
            var wordPrototype = ObjectCopier.Clone(appConfig.Elements.OfType<Word>().Single());
            var wordIds = Sentence.Words.Select(w => new Pair
            {
                Id = int.Parse(w.GetAttributeByName("id")),
                Form = w.GetAttributeByName("form")
            }).ToList();
            var addWordWindow = new AddWordWindow(new AddWordViewModel(wordPrototype, wordIds));

            if (addWordWindow.ShowDialog().GetValueOrDefault())
            {
                var word = ((AddWordViewModel) addWordWindow.DataContext).Word;
                Sentence.Words.Add(word);

                var wordReorderingWindow = new WordReorderingWindow(new WordReorderingViewModel(Sentence));
                if (wordReorderingWindow.ShowDialog().GetValueOrDefault())
                {
                }

                //todo: ensure tree and id ordering is in place

                Words.Add(new WordEditorViewModel(word, EventAggregator));
                EventAggregator.GetEvent<AddWordVertexEvent>().Publish(word);
            }
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
            EventAggregator.GetEvent<SetSentenceEditModeEvent>().Publish(new SetSenteceGraphOperationModeRequest
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

        public void Initialize()
        {
            BuildSentenceGraph();
            SetupGraphLogic();
        }

        private void SetupGraphLogic()
        {
            var logicCore = new SentenceGxLogicCore
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama,
                EdgeCurvingEnabled = false
            };

            var parameters =
                SentenceGraphLogicCore.AlgorithmFactory.CreateLayoutParameters(
                    LayoutAlgorithmTypeEnum.EfficientSugiyama) as EfficientSugiyamaLayoutParameters;

            if (parameters != null)
            {
                parameters.EdgeRouting = SugiyamaEdgeRoutings.Orthogonal;
                parameters.LayerDistance = parameters.VertexDistance = 50;
                logicCore.EdgeCurvingEnabled = false;
                logicCore.DefaultLayoutAlgorithmParams = parameters;
            }

            logicCore.Graph = sentenceGraph;

            SentenceGraphLogicCore = logicCore;
            SelectedLayoutAlgorithmType = LayoutAlgorithmTypeEnum.EfficientSugiyama;
            SelectedEdgeRoutingAlgorithmType = EdgeRoutingAlgorithmTypeEnum.None;
        }

        private void BuildSentenceGraph()
        {
            sentenceGraph = new SentenceGraph();

            int id, headId;

            foreach (var word in Sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word));
            }

            var vertices = sentenceGraph.Vertices.ToList();

            foreach (var word in Sentence.Words)
            {
                if (int.TryParse(word.GetAttributeByName("head"), out headId))
                {
                    if (headId == 0)
                    {
                        continue;
                    }

                    if (int.TryParse(word.GetAttributeByName("id"), out id))
                    {
                        var wordVertex = vertices.Single(v => v.ID == id);
                        var headWordVertex = vertices.Single(v => v.ID == headId);

                        sentenceGraph.AddEdge(
                            new WordEdge(headWordVertex, wordVertex) {Text = word.GetAttributeByName("deprel")});
                    }
                }
            }
        }
    }
}