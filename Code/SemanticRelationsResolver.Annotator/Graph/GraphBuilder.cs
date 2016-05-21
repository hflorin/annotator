namespace SemanticRelationsResolver.Annotator.Graph
{
    using System;
    using System.Linq;
    using Domain.Configuration;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
    using Wrapper;

    public class GraphBuilder
    {
        private IAppConfig appConfig;
        private readonly Definition definition;

        public GraphBuilder(IAppConfig appConfig, Definition definition = null)
        {
            if (appConfig == null)
            {
                throw new ArgumentNullException("appConfig");
            }

            if (definition == null)
            {
                if (appConfig.Definitions.Any())
                {
                    this.definition = appConfig.Definitions.First();
                }
                else
                {
                    this.definition = MotherObjects.DefaultDefinition;
                }
            }

            this.appConfig = appConfig;
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
                parameters.EdgeRouting = SugiyamaEdgeRoutings.Orthogonal;
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

            int to, from;

            foreach (var word in sentence.Words)
            {
                sentenceGraph.AddVertex(new WordVertex(word));
            }

            var vertices = sentenceGraph.Vertices.ToList();

            foreach (var word in sentence.Words)
            {
                if (int.TryParse(word.GetAttributeByName(definition.Vertex.FromAttributeName), out from))
                {
                    if (from == 0)
                    {
                        continue;
                    }

                    if (int.TryParse(word.GetAttributeByName(definition.Vertex.ToAttributeName), out to))
                    {
                        var toWordVertex = vertices.Single(v => v.ID == to);
                        var fromWordVertex = vertices.Single(v => v.ID == from);

                        sentenceGraph.AddEdge(
                            new WordEdge(fromWordVertex, toWordVertex)
                            {
                                Text = word.GetAttributeByName(definition.Edge.LabelAttributeName)
                            });
                    }
                }
            }

            return sentenceGraph;
        }
    }
}