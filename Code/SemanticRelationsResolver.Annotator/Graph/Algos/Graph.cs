namespace SemanticRelationsResolver.Annotator.Graph.Algos
{
    using System.Collections.Generic;

    public class Graph
    {
        private readonly IDictionary<int, IList<int>> adjencyList;
        private readonly int verticesNumber;

        public Graph(int numberOfVertices)
        {
            verticesNumber = numberOfVertices;
            adjencyList = new Dictionary<int, IList<int>>(verticesNumber);
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

        public bool IsTree()
        {
            var visited = new bool[verticesNumber];

            if (HasCycle(0, visited, -1))
            {
                return false;
            }

            foreach (var b in visited)
            {
                if (!b)
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasCycle(int vertex, bool[] visited, int parent)
        {
            visited[vertex] = true;

            foreach (var v in adjencyList[vertex])
            {
                if (!visited[v])
                {
                    if (HasCycle(v, visited, vertex))
                    {
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