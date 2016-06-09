namespace Treebank.Annotator.Graph
{
    using System.Linq;
    using Mappers.Configuration;
    using Wrapper;

    public class GraphBuilder
    {
        public GraphBuilder(IAppConfig appConfig, Definition definition = null)
        {
            if (definition == null)
            {
                if ((appConfig != null) && appConfig.Definitions.Any())
                {
                    CurrentDefinition = appConfig.Definitions.First();
                }
                else
                {
                    CurrentDefinition = MotherObjects.DefaultDefinition;
                }
            }
            else
            {
                CurrentDefinition = definition;
            }
        }

        public Definition CurrentDefinition { get; set; }

        public SentenceGxLogicCore SetupGraphLogic(SentenceWrapper sentence)
        {
            var sentenceGraph = BuildSentenceGraph(sentence);

            return new SentenceGxLogicCore {EdgeCurvingEnabled = false, Graph = sentenceGraph, EnableParallelEdges = true};
        }

        private SentenceGraph BuildSentenceGraph(SentenceWrapper sentence)
        {
            var sentenceGraph = new SentenceGraph();

            string to, from;

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word, CurrentDefinition.Vertex.LabelAttributeName));
            }

            var vertices = sentenceGraph.Vertices.ToList();

            foreach (var word in sentence.Words)
            {
                from = word.GetAttributeByName(CurrentDefinition.Vertex.FromAttributeName);

                if (from == "0")
                {
                    continue;
                }

                to = word.GetAttributeByName(CurrentDefinition.Vertex.ToAttributeName);

                var toWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentDefinition.Vertex.ToAttributeName).Equals(to));
                var fromWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentDefinition.Vertex.ToAttributeName).Equals(from));
                if ((toWordVertex != null) && (fromWordVertex != null))
                {
                    sentenceGraph.AddEdge(
                        new WordEdge(fromWordVertex, toWordVertex)
                        {
                            Text = word.GetAttributeByName(CurrentDefinition.Edge.LabelAttributeName),
                            SourceConnectionPointId = 1,
                            TargetConnectionPointId = 1
                        });
                }
            }

            return sentenceGraph;
        }
    }
}