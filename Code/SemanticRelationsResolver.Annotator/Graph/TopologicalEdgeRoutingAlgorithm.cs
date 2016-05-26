namespace SemanticRelationsResolver.Annotator.Graph
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

    public class TopologicalEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> :
        EdgeRoutingAlgorithmBase<TVertex, TEdge, TGraph>
        where TGraph : class, IMutableBidirectionalGraph<TVertex, TEdge>
        where TEdge : class, IGraphXEdge<TVertex>
        where TVertex : class, IGraphXVertex, IIdentifiableGraphDataObject
    {
        public TopologicalEdgeRoutingAlgorithm(
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

            var edgesSortedByDistanceBetweenVertices = distancesBetweenEdgeVertices.ToList();
            edgesSortedByDistanceBetweenVertices.Sort((left, right) => left.Value.CompareTo(right.Value));


            var numberOfEdgesPerVertex = new Dictionary<TVertex, int>();
            var numberOfEdgesDrawnPerVertex = new Dictionary<TVertex, int>();

            foreach (var edge in edgesSortedByDistanceBetweenVertices)
            {
                if (numberOfEdgesPerVertex.ContainsKey(edge.Key.Source))
                {
                    numberOfEdgesPerVertex[edge.Key.Source]++;
                }
                else
                {
                    numberOfEdgesPerVertex.Add(edge.Key.Source, 1);
                }

                if (numberOfEdgesPerVertex.ContainsKey(edge.Key.Target))
                {
                    numberOfEdgesPerVertex[edge.Key.Target]++;
                }
                else
                {
                    numberOfEdgesPerVertex.Add(edge.Key.Target, 1);
                }

                numberOfEdgesDrawnPerVertex[edge.Key.Source] = 0;
                numberOfEdgesDrawnPerVertex[edge.Key.Target] = 0;
            }

            var offset = -10;
            foreach (var item in edgesSortedByDistanceBetweenVertices)
            {
                ComputeEdgeRoutePoints(item.Key, offset, cancellationToken, numberOfEdgesPerVertex, numberOfEdgesDrawnPerVertex);
                offset -= 10;
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

        private void ComputeEdgeRoutePoints(TEdge edge, double offset, CancellationToken cancellationToken, IDictionary<TVertex, int> edgesPerVertex, IDictionary<TVertex, int> edgesDrawnPerVertex)
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

            var startPoint = VertexPositions[edge.Source];

            var endPoint = VertexPositions[edge.Target];

            if (startPoint == endPoint)
            {
                return;
            }

            var sourceVertexWidth = VertexSizes[edge.Source];
            var targetVertexWidth = VertexSizes[edge.Target];

            var adjustedStartPoint =
                new Point(
                    startPoint.X +
                    ((sourceVertexWidth.Width/edgesPerVertex[edge.Source])*edgesDrawnPerVertex[edge.Source]),
                    startPoint.Y);

            var adjustedEndPoint =
                new Point(
                    endPoint.X +
                    ((targetVertexWidth.Width/edgesPerVertex[edge.Target])*edgesDrawnPerVertex[edge.Target]),
                    endPoint.Y);

            var tempList = new List<Point> { adjustedStartPoint };

            tempList.Add(new Point(startPoint.X + ((sourceVertexWidth.Width/edgesPerVertex[edge.Source]) * edgesDrawnPerVertex[edge.Source]), startPoint.Y + offset));

            tempList.Add(new Point(endPoint.X + ((targetVertexWidth.Width/edgesPerVertex[edge.Target])*edgesDrawnPerVertex[edge.Target]), startPoint.Y + offset));

            tempList.Add(adjustedEndPoint);

            if (EdgeRoutes.ContainsKey(edge))
            {
                EdgeRoutes[edge] = tempList.Count > 2 ? tempList.ToArray() : null;
            }
            else
            {
                EdgeRoutes.Add(edge, tempList.Count > 2 ? tempList.ToArray() : null);
            }

            edgesDrawnPerVertex[edge.Source]++;
            edgesDrawnPerVertex[edge.Target]++;
        }
    }
}