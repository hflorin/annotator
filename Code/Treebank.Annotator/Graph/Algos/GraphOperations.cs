namespace Treebank.Annotator.Graph.Algos
{
    using System.Collections.Generic;
    using Mappers.Configuration;
    using Wrapper;

    public static class GraphOperations
    {
        public static Graph GetGraph(SentenceWrapper sentence, Definition definition)
        {
            var result = new Graph(sentence.Words.Count);

            var wordToVertexMapping = new Dictionary<string, int>();

            var vertexId = 0;

            foreach (var word in sentence.Words)
            {
                wordToVertexMapping.Add(word.GetAttributeByName("id"), vertexId++);
            }

            foreach (var word in sentence.Words)
            {
                var from = word.GetAttributeByName(definition.Vertex.FromAttributeName);

                if (from == "0")
                {
                    continue;
                }

                var to = word.GetAttributeByName(definition.Vertex.ToAttributeName);

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