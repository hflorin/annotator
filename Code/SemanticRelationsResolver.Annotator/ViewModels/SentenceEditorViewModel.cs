namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System.Linq;
    using Graph;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
    using Prism.Events;
    using Wrapper;

    public class SentenceEditorViewModel : Observable
    {
        private IEventAggregator eventAggregator;
        private SentenceGraph sentenceGraph;

        private SentenceGxLogicCore sentenceLogicCore;

        public SentenceEditorViewModel(
            IEventAggregator eventAggregator,
            SentenceWrapper sentence)
        {
            this.eventAggregator = eventAggregator;
            Sentence = sentence;
            sentenceGraph = new SentenceGraph();
            sentenceLogicCore = new SentenceGxLogicCore();
            sentenceLogicCore.Graph = sentenceGraph;
        }

        public SentenceWrapper Sentence { get; set; }

        public SentenceGxLogicCore SentenceGraphLogicCore
        {
            get { return sentenceLogicCore; }
            set
            {
                sentenceLogicCore = value;
                OnPropertyChanged();
            }
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
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama
            };


            logicCore.DefaultLayoutAlgorithmParams =
                logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.EfficientSugiyama);

            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;

            logicCore.DefaultOverlapRemovalAlgorithmParams =
                logicCore.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters) logicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters) logicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            logicCore.AsyncAlgorithmCompute = false;

            logicCore.Graph = sentenceGraph;

            SentenceGraphLogicCore = logicCore;
        }

        private void BuildSentenceGraph()
        {
            sentenceGraph = new SentenceGraph();

            foreach (var word in Sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex {ID = word.Id, Text = word.Form});
            }

            var vlist = sentenceGraph.Vertices.ToList();

            foreach (var word in Sentence.Words)
            {
                if (word.HeadWordId == 0)
                {
                    continue;
                }

                var wordVertex = vlist.Single(v => v.ID == word.Id);
                var headWordVertex = vlist.Single(v => v.ID == word.HeadWordId);

                sentenceGraph.AddEdge(new WordEdge(headWordVertex, wordVertex) {Text = word.DependencyRelation});
            }
        }
    }
}