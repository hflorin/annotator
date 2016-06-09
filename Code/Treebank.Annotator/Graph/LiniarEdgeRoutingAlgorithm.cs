namespace Treebank.Annotator.Graph
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using GraphX.Measure;
    using GraphX.PCL.Common;
    using GraphX.PCL.Common.Exceptions;
    using GraphX.PCL.Common.Interfaces;
    using GraphX.PCL.Logic.Algorithms.EdgeRouting;
    using QuickGraph;

    public class LiniarEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> :
        EdgeRoutingAlgorithmBase<TVertex, TEdge, TGraph>
        where TGraph : class, IMutableBidirectionalGraph<TVertex, TEdge>
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex, IIdentifiableGraphDataObject
    {
        public LiniarEdgeRoutingAlgorithm(
            TGraph graph,
            IDictionary<TVertex, Point> vertexPositions = null,
            IDictionary<TVertex, Rect> vertexSizes = null,
            IEdgeRoutingParameters parameters = null)
            : base(graph, vertexPositions, vertexSizes, parameters)
        {
        }

        public override void UpdateVertexData(TVertex vertex, Point position, Rect size)
        {
            VertexPositions.AddOrUpdate(vertex, position);
            VertexSizes.AddOrUpdate(vertex, size);
        }

        public override Point[] ComputeSingle(TEdge edge)
        {
            return EdgeRoutes.ContainsKey(edge) ? EdgeRoutes[edge] : null;
        }

        public override void Compute(CancellationToken cancellationToken)
        {
            EdgeRoutes.Clear();

            var distancesBetweenEdgeVertices = ComputeDistancesBetweenEdgeVertices();

            var edgeGaps = distancesBetweenEdgeVertices.ToList();
            var sortedEdgeGaps = edgeGaps.ToList();
            sortedEdgeGaps.Sort((left, right) => left.Value.CompareTo(right.Value));

            var offset = -25;
            for (var i = 0; i < sortedEdgeGaps.Count; i++)
            {
                ComputeEdgeRoutePoints(
                    sortedEdgeGaps[i].Key,
                    offset,
                    cancellationToken);

                if ((i + 1 < sortedEdgeGaps.Count) &&
                    (sortedEdgeGaps[i].Value != sortedEdgeGaps[i + 1].Value))
                {
                    offset -= 25;
                }
            }
        }

        private Dictionary<TEdge, double> ComputeDistancesBetweenEdgeVertices()
        {
            var distancesBetweenEdgeNodes = new Dictionary<TEdge, double>();

            foreach (var edge in Graph.Edges)
            {
                var fromPoint = VertexPositions[edge.Source];
                var toPoint = VertexPositions[edge.Target];

                var distance = (fromPoint.X - toPoint.X)*(fromPoint.X - toPoint.X)
                               + (fromPoint.X - toPoint.X)*(fromPoint.X - toPoint.X);

                distancesBetweenEdgeNodes.Add(edge, distance);
            }

            return distancesBetweenEdgeNodes;
        }

        private void ComputeEdgeRoutePoints(
            TEdge edge,
            double offset,
            CancellationToken cancellationToken)
        {
            if ((edge.Source.ID == -1) || (edge.Target.ID == -1))
            {
                throw new GX_InvalidDataException(
                    "SimpleEdgeRouting() -> You must assign unique ID for each vertex to use SimpleER algo!");
            }

            if ((edge.Source.ID == edge.Target.ID) || !VertexPositions.ContainsKey(edge.Target))
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            ComputeEdgePoints(edge, offset);
        }

        private void ComputeEdgePoints(
            TEdge edge,
            double offset)
        {
            var sourcePoint = VertexPositions[edge.Source];

            var targetPoint = VertexPositions[edge.Target];

            if (sourcePoint == targetPoint)
            {
                return;
            }

            var sourceVertexWidth = VertexSizes[edge.Source].Width;
            var targetVertexWidth = VertexSizes[edge.Target].Width;

            var x = sourcePoint.X;
            var y = sourcePoint.Y;

            var tempList = new List<Point> {new Point(x, y)};

            x = sourcePoint.X;
            y = sourcePoint.Y + offset;
            tempList.Add(new Point(x, y));

            x = targetPoint.X;
            y = sourcePoint.Y + offset;
            tempList.Add(new Point(x, y));

            x = targetPoint.X;
            y = targetPoint.Y;

            tempList.Add(new Point(x, y));

            if (EdgeRoutes.ContainsKey(edge))
            {
                EdgeRoutes[edge] = tempList.Count > 2 ? tempList.ToArray() : null;
            }
            else
            {
                EdgeRoutes.Add(edge, tempList.Count > 2 ? tempList.ToArray() : null);
            }
        }
    }
}