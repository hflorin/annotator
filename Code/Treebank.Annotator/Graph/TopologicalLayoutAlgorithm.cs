namespace Treebank.Annotator.Graph
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using GraphX.Measure;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Common.Interfaces;
    using QuickGraph;

    public class TopologicalLayoutAlgorithm<TVertex, TEdge, TGraph> : IExternalLayout<TVertex, TEdge>
        where TVertex : class, IIdentifiableGraphDataObject
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>, IMutableVertexAndEdgeListGraph<TVertex, TEdge>
    {
        private readonly TGraph graph;

        private readonly double vertexPositionXOffset;

        private readonly double vertexPositionYOffset;

        public TopologicalLayoutAlgorithm(TGraph graph, double vertexXOffset = 0.0d, double vertexYOffset = 0.0d)
        {
            this.graph = graph;
            vertexPositionXOffset = vertexXOffset;
            vertexPositionYOffset = vertexYOffset;
            VertexPositions = new Dictionary<TVertex, Point>();
            VertexSizes = new Dictionary<TVertex, Size>();
        }

        public bool NeedVertexSizes
        {
            get { return true; }
        }

        public bool SupportsObjectFreeze { get; private set; }

        public IDictionary<TVertex, Point> VertexPositions { get; private set; }

        public IDictionary<TVertex, Size> VertexSizes { get; set; }

        public void Compute(CancellationToken cancellationToken)
        {
            var usableVertices = graph.Vertices.Where(v => v.SkipProcessing != ProcessingOptionEnum.Freeze).ToList();

            if (VertexPositions.Count == 0)
            {
                foreach (var item in graph.Vertices.Where(v => v.SkipProcessing == ProcessingOptionEnum.Freeze))
                {
                    VertexPositions.Add(item, new Point());
                }
            }

            var startingPoint = new Point(0.0d, 0.0d);

            foreach (var vertex in usableVertices)
            {
                cancellationToken.ThrowIfCancellationRequested();

                VertexPositions[vertex] = startingPoint;

                var vertexWidth = VertexSizes[vertex].Width;
                var vertexHeight = VertexSizes[vertex].Height;

                if (vertexPositionXOffset != 0.0d)
                {
                    startingPoint.X += vertexWidth + vertexPositionXOffset;
                }

                if (vertexPositionYOffset != 0.0d)
                {
                    startingPoint.Y += vertexHeight + vertexPositionYOffset;
                }
            }
        }

        public void ResetGraph(IEnumerable<TVertex> vertices, IEnumerable<TEdge> edges)
        {
        }
    }
}