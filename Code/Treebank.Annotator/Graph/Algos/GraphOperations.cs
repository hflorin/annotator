namespace Treebank.Annotator.Graph.Algos
{
    using System.Collections.Generic;
    using Mappers.Configuration;
    using Prism.Events;
    using Wrapper;

    public static class GraphOperations
    {
        public static Graph GetGraph(SentenceWrapper sentence, Definition definition, IEventAggregator eventAggregator)
        {
            var wordToVertexMapping = new Dictionary<string, int>();

            var vertexId = 0;

            foreach (var word in sentence.Words)
            {
                wordToVertexMapping.Add(word.GetAttributeByName(definition.Edge.TargetVertexAttributeName), vertexId++);
            }

            var result = new Graph(sentence.Words.Count, wordToVertexMapping);

            foreach (var word in sentence.Words)
            {
                var from = word.GetAttributeByName(definition.Edge.SourceVertexAttributeName);

                if ((from == null) || (from == "0"))
                {
                    continue;
                }

                var to = word.GetAttributeByName(definition.Edge.TargetVertexAttributeName);

                if (to == null)
                {
                    continue;
                }

                if (wordToVertexMapping.ContainsKey(to) && wordToVertexMapping.ContainsKey(from))
                {
                    var toVertexId = wordToVertexMapping[to];
                    var fromVertexId = wordToVertexMapping[from];

                    result.AddEdge(fromVertexId, toVertexId);
                }
            }

            return result;
        }
    }
}