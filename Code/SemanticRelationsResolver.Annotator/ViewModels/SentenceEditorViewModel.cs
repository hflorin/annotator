namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System.Linq;
    using Graph;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
    using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
    using Prism.Events;
    using Wrapper;

    public class SentenceEditorViewModel : Observable
    {
        private IEventAggregator eventAggregator;

        public SentenceEditorViewModel(
            IEventAggregator eventAggregator,
            SentenceWrapper sentence)
        {
            this.eventAggregator = eventAggregator;
            Sentence = sentence;
            var graph = BuildSentenceGraph();
            SetupGraphLogic(graph);
        }

        public SentenceWrapper Sentence { get; set; }

        public SentenceGxLogicCore Graph { get; set; }

        private void SetupGraphLogic(SentenceGraph graph)
        {
            var LogicCore = new SentenceGxLogicCore();
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            LogicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama;
            //Now we can set optional parameters using AlgorithmFactory
            //NOTE: default parameters can be automatically created each time you change Default algorithms
            LogicCore.DefaultLayoutAlgorithmParams =
                LogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.EfficientSugiyama);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            //((KKLayoutParameters) LogicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            LogicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Setup optional params
            LogicCore.DefaultOverlapRemovalAlgorithmParams =
                LogicCore.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters) LogicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters) LogicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            LogicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            LogicCore.AsyncAlgorithmCompute = false;

            //Finally assign logic core to GraphArea object
            LogicCore.Graph = graph;

            Graph = LogicCore;
        }

        private SentenceGraph BuildSentenceGraph()
        {
            //Create data graph object
            var graph = new SentenceGraph();

            //Create and add vertices using some DataSource for ID's
            foreach (var word in Sentence.Words)
            {
                graph.AddVertex(new WordVertex {ID = word.Id, Text = word.Form});
            }

            var vlist = graph.Vertices.ToList();
            //Generate random edges for the vertices
            foreach (var word in Sentence.Words)
            {
                if (word.HeadWordId == 0)
                {
                    continue;
                }

                var wordVertex = vlist.Single(v => v.ID == word.Id);
                var headWordVertex = vlist.Single(v => v.ID == word.HeadWordId);

                graph.AddEdge(new WordEdge(headWordVertex, wordVertex) {Text = word.DependencyRelation});
            }

            return graph;
        }
    }
}