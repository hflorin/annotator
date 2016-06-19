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

            return new SentenceGxLogicCore
            {
                EdgeCurvingEnabled = false,
                Graph = sentenceGraph,
                EnableParallelEdges = false
            };
        }

        private SentenceGraph BuildSentenceGraph(SentenceWrapper sentence)
        {
            var sentenceGraph = new SentenceGraph();

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word, CurrentDefinition.Vertex.LabelAttributeName));
            }

            AddEdges(sentence, sentenceGraph);

            return sentenceGraph;
        }

        public SentenceGxLogicCore SetupGraphLogic(SentenceWrapper sentence, SentenceWrapper rightSentence)
        {
            var sentenceGraph = BuildSentenceGraph(sentence, rightSentence);

            return new SentenceGxLogicCore
            {
                EdgeCurvingEnabled = false,
                Graph = sentenceGraph,
                EnableParallelEdges = false
            };
        }

        private SentenceGraph BuildSentenceGraph(SentenceWrapper sentence, SentenceWrapper rightSentence)

        {
            var sentenceGraph = new SentenceGraph();

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word, CurrentDefinition.Vertex.LabelAttributeName));
            }

            AddComparableEdges(sentence, sentenceGraph, true, false);
            AddComparableEdges(rightSentence, sentenceGraph, false, true);

            return sentenceGraph;
        }

        private void AddComparableEdges(SentenceWrapper sentence, SentenceGraph sentenceGraph, bool isLeft, bool isRight)
        {
            string to, from;
            var vertices = sentenceGraph.Vertices.ToList();
            foreach (var word in sentence.Words)
            {
                from = word.GetAttributeByName(CurrentDefinition.Edge.SourceVertexAttributeName);

                if (from == "0")
                {
                    continue;
                }

                to = word.GetAttributeByName(CurrentDefinition.Edge.TargetVertexAttributeName);

                var toWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentDefinition.Edge.TargetVertexAttributeName).Equals(to));
                var fromWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentDefinition.Edge.TargetVertexAttributeName).Equals(from));
                if ((toWordVertex != null) && (fromWordVertex != null))
                {
                    sentenceGraph.AddEdge(
                        new OrderedWordEdge(fromWordVertex, toWordVertex)
                        {
                            Text = word.GetAttributeByName(CurrentDefinition.Edge.LabelAttributeName),
                            SourceConnectionPointId = 1,
                            TargetConnectionPointId = 1,
                            IsLeft = isLeft,
                            IsRight = isRight
                        });
                }
            }
        }

        private void AddEdges(SentenceWrapper sentence, SentenceGraph sentenceGraph)
        {
            string to, from;
            var vertices = sentenceGraph.Vertices.ToList();
            foreach (var word in sentence.Words)
            {
                from = word.GetAttributeByName(CurrentDefinition.Edge.SourceVertexAttributeName);

                if (from == "0")
                {
                    continue;
                }

                to = word.GetAttributeByName(CurrentDefinition.Edge.TargetVertexAttributeName);

                var toWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentDefinition.Edge.TargetVertexAttributeName).Equals(to));
                var fromWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentDefinition.Edge.TargetVertexAttributeName).Equals(from));
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
        }
    }
}