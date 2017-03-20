namespace Treebank.Annotator.Graph
{
    using System.Linq;
    using System.Threading.Tasks;
    using Mappers;
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
                    CurrentGraphDefinition = appConfig.Definitions.First();
                }
                else
                {
                    CurrentGraphDefinition = MotherObjects.DefaultDefinition;
                }
            }
            else
            {
                CurrentGraphDefinition = definition;
            }
        }

        public Definition CurrentGraphDefinition { get; set; }

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

            sentence.Words.Sort(
                    (l, r) =>
                        int.Parse(l.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName))
                            .CompareTo(int.Parse(r.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName))));

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word, CurrentGraphDefinition.Vertex.LabelAttributeName));
            }

            AddEdges(sentence, sentenceGraph);

            return sentenceGraph;
        }

        public async Task<SentenceGxLogicCore> SetupGraphLogic(SentenceWrapper sentence, SentenceWrapper rightSentence, IAppConfigMapper appConfigMapper, string leftSentenceConfigFilePath, string rightSentenceConfigFilePath)
        {
            var sentenceGraph = await BuildSentenceGraph(sentence, rightSentence, appConfigMapper, leftSentenceConfigFilePath, rightSentenceConfigFilePath);

            return new SentenceGxLogicCore
            {
                EdgeCurvingEnabled = false,
                Graph = sentenceGraph,
                EnableParallelEdges = false
            };
        }

        private async Task<SentenceGraph> BuildSentenceGraph(SentenceWrapper sentence, SentenceWrapper rightSentence, IAppConfigMapper appConfigMapper, string leftSentenceConfigFilePath, string rightSentenceConfigFilePath)
        {
            if (sentence.Words.Count != rightSentence.Words.Count)
            {
                return new SentenceGraph();
            }

            var leftAppConfig = await appConfigMapper.Map(leftSentenceConfigFilePath);
            IAppConfig rightAppConfig;

            if (leftSentenceConfigFilePath.Equals(rightSentenceConfigFilePath))
            {
                rightAppConfig = leftAppConfig;
            }
            else
            {
                rightAppConfig = await appConfigMapper.Map(rightSentenceConfigFilePath);
            }

            if (leftAppConfig == null || rightAppConfig == null)
            {
                return new SentenceGraph();
            }

            var sentenceGraph = new SentenceGraph();

            CurrentGraphDefinition = leftAppConfig.Definitions.FirstOrDefault();

            if (CurrentGraphDefinition == null)
            {
                return new SentenceGraph();
            }

            sentence.Words.Sort(
                    (l, r) =>
                        int.Parse(l.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName))
                            .CompareTo(int.Parse(r.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName))));

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word, CurrentGraphDefinition.Vertex.LabelAttributeName));
            }
            
            AddComparableEdges(sentence, sentenceGraph, true, false);

            CurrentGraphDefinition = rightAppConfig.Definitions.FirstOrDefault();
            AddComparableEdges(rightSentence, sentenceGraph, false, true);

            return sentenceGraph;
        }

        private void AddComparableEdges(SentenceWrapper sentence, SentenceGraph sentenceGraph, bool isLeft, bool isRight)
        {
            string to, from;
            var vertices = sentenceGraph.Vertices.ToList();
            foreach (var word in sentence.Words)
            {
                from = word.GetAttributeByName(CurrentGraphDefinition.Edge.SourceVertexAttributeName);

                if (from == "0")
                {
                    continue;
                }

                to = word.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName);

                var toWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName).Equals(to));
                var fromWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName).Equals(from));
                if ((toWordVertex != null) && (fromWordVertex != null))
                {
                    sentenceGraph.AddEdge(
                        new OrderedWordEdge(fromWordVertex, toWordVertex)
                        {
                            Text = word.GetAttributeByName(CurrentGraphDefinition.Edge.LabelAttributeName),
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
                from = word.GetAttributeByName(CurrentGraphDefinition.Edge.SourceVertexAttributeName);

                if (from == "0")
                {
                    continue;
                }

                to = word.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName);

                var toWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName).Equals(to));
                var fromWordVertex =
                    vertices.FirstOrDefault(
                        v => v.WordWrapper.GetAttributeByName(CurrentGraphDefinition.Edge.TargetVertexAttributeName).Equals(from));
                if ((toWordVertex != null) && (fromWordVertex != null))
                {
                    sentenceGraph.AddEdge(
                        new WordEdge(fromWordVertex, toWordVertex)
                        {
                            Text = word.GetAttributeByName(CurrentGraphDefinition.Edge.LabelAttributeName),
                            SourceConnectionPointId = 1,
                            TargetConnectionPointId = 1
                        });
                }
            }
        }
    }
}