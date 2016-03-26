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

        private SentenceWrapper sentenceWrapper;

        public SentenceEditorViewModel(IEventAggregator eventAggregator, SentenceWrapper sentence)
        {
            this.eventAggregator = eventAggregator;
            Sentence = sentence;
            sentenceGraph = new SentenceGraph();
            sentenceLogicCore = new SentenceGxLogicCore();
            sentenceLogicCore.Graph = sentenceGraph;
        }

        public SentenceWrapper Sentence
        {
            get { return sentenceWrapper; }
            set
            {
                sentenceWrapper = value;
                OnPropertyChanged();
            }
        }

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

            int id, headId;

            foreach (var word in Sentence.Words)
            {
                id = int.Parse(word.Attributes.Single(a => a.Name.Equals("id")).Value);
                sentenceGraph.AddVertex(
                    new WordVertex {ID = id, Text = word.Attributes.Single(a => a.Name.Equals("form")).Value});
            }

            var vlist = sentenceGraph.Vertices.ToList();

            foreach (var word in Sentence.Words)
            {
                if (int.TryParse(word.Attributes.Single(a => a.Name.Equals("head")).Value, out headId))
                {
                    if (headId == 0)
                    {
                        continue;
                    }

                    if (int.TryParse(word.Attributes.Single(a => a.Name.Equals("id")).Value, out id))
                    {
                        id = int.Parse(word.Attributes.Single(a => a.Name.Equals("id")).Value);

                        var wordVertex = vlist.Single(v => v.ID == id);
                        var headWordVertex = vlist.Single(v => v.ID == headId);

                        sentenceGraph.AddEdge(
                            new WordEdge(headWordVertex, wordVertex)
                            {
                                Text =
                                    word.Attributes.Single(
                                        a => a.Name.Equals("deprel")).Value
                            });
                    }
                }
            }
        }
    }
}