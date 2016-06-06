namespace Treebank.Annotator.Graph.Algos
{
    using System.Collections.Generic;
    using System.Linq;

    public class Graph
    {
        private readonly IDictionary<int, IList<int>> adjencyList;
        private readonly int verticesNumber;
        private readonly Dictionary<string, int> wordToVertexMapping;

        public Graph(int numberOfVertices, Dictionary<string, int> wordToVertexMapping = null)
        {
            verticesNumber = numberOfVertices;
            adjencyList = new Dictionary<int, IList<int>>(verticesNumber);
            this.wordToVertexMapping = wordToVertexMapping;
        }

        public void AddEdge(int fromVertex, int toVertex)
        {
            if (!adjencyList.ContainsKey(fromVertex))
            {
                adjencyList[fromVertex] = new List<int>();
            }
            if (!adjencyList.ContainsKey(toVertex))
            {
                adjencyList[toVertex] = new List<int>();
            }

            adjencyList[fromVertex].Add(toVertex);
            adjencyList[toVertex].Add(fromVertex);
        }

        public bool IsTree(CheckGraphResult validation)
        {
            if (verticesNumber <= 1)
            {
                return true;
            }

            var visited = new bool[verticesNumber];

            if (HasCycle(0, visited, -1, validation))
            {
                return false;
            }

            for (var i = 0; i < visited.Length; i++)
            {
                if (!visited[i])
                {
                    validation.DisconnectedWordIds.Add(wordToVertexMapping.Single(p => p.Value == i).Key);
                    return false;
                }
            }

            return true;
        }

        private bool HasCycle(int vertex, bool[] visited, int parent, CheckGraphResult validation)
        {
            visited[vertex] = true;

            if (!adjencyList.ContainsKey(vertex))
            {
                return false;
            }

            foreach (var v in adjencyList[vertex])
            {
                if (!visited[v])
                {
                    if (HasCycle(v, visited, vertex, validation))
                    {
                        var cycle = new List<string>();

                        for (var i = 0; i < visited.Length; i++)
                        {
                            if (visited[i])
                                cycle.Add(wordToVertexMapping.Single(p => p.Value == i).Key);
                        }

                        cycle.Add(wordToVertexMapping.Single(p => p.Value == v).Key);

                        validation.Cycles.Add(cycle);
                        return true;
                    }
                }
                else if (v != parent)
                {
                    return true;
                }
            }

            return false;
        }
    }
}