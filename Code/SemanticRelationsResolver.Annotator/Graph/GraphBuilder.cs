namespace Treebank.Annotator.Graph
{
    using System.Linq;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
    using SemanticRelationsResolver.Annotator.Graph;
    using SemanticRelationsResolver.Annotator.Wrapper;
    using SemanticRelationsResolver.Domain.Configuration;

    public class GraphBuilder
    {
        private readonly Definition definition;

        public GraphBuilder(IAppConfig appConfig, Definition definition = null)
        {
            if (definition == null)
            {
                if ((appConfig != null) && appConfig.Definitions.Any())
                {
                    this.definition = appConfig.Definitions.First();
                }
                else
                {
                    this.definition = MotherObjects.DefaultDefinition;
                }
            }
            else
            {
                this.definition = definition;
            }
        }

        public SentenceGxLogicCore SetupGraphLogic(SentenceWrapper sentence)
        {
            var sentenceGraph = BuildSentenceGraph(sentence);

            var logicCore = new SentenceGxLogicCore
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.EfficientSugiyama,
                EdgeCurvingEnabled = false
            };

            var parameters =
                logicCore.AlgorithmFactory.CreateLayoutParameters(
                    LayoutAlgorithmTypeEnum.EfficientSugiyama) as EfficientSugiyamaLayoutParameters;

            if (parameters != null)
            {
                parameters.EdgeRouting = SugiyamaEdgeRoutings.Traditional;
                parameters.LayerDistance = parameters.VertexDistance = 50;
                logicCore.EdgeCurvingEnabled = false;
                logicCore.DefaultLayoutAlgorithmParams = parameters;
            }

            logicCore.Graph = sentenceGraph;
            return logicCore;
        }

        private SentenceGraph BuildSentenceGraph(SentenceWrapper sentence)
        {
            var sentenceGraph = new SentenceGraph();

            string to, from;

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word));
            }

            var vertices = sentenceGraph.Vertices.ToList();

            foreach (var word in sentence.Words)
            {
                from = word.GetAttributeByName(definition.Vertex.FromAttributeName);

                if (from == "0")
                {
                    continue;
                }

                to = word.GetAttributeByName(definition.Vertex.ToAttributeName);

                var toWordVertex =
                    vertices.Single(v => v.WordWrapper.GetAttributeByName(definition.Vertex.ToAttributeName).Equals(to));
                var fromWordVertex =
                    vertices.Single(
                        v => v.WordWrapper.GetAttributeByName(definition.Vertex.ToAttributeName).Equals(from));

                sentenceGraph.AddEdge(
                    new WordEdge(fromWordVertex, toWordVertex)
                    {
                        Text = word.GetAttributeByName(definition.Edge.LabelAttributeName)
                    });
            }

            return sentenceGraph;
        }
    }
}