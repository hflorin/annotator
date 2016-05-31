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

                numberOfEdgesDrawnPerVertex[edge.Key.Source] = 1;
                numberOfEdgesDrawnPerVertex[edge.Key.Target] = 1;
            }

            var offset = -10;
            foreach (var item in edgesSortedByDistanceBetweenVertices)
            {
                ComputeEdgeRoutePoints(
                    item.Key, 
                    offset, 
                    cancellationToken, 
                    numberOfEdgesPerVertex, 
                    numberOfEdgesDrawnPerVertex);
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

                var distance = ((fromPoint.X - toPoint.X) * (fromPoint.X - toPoint.X))
                               + ((fromPoint.X - toPoint.X) * (fromPoint.X - toPoint.X));

                distancesBetweenEdgeNodes.Add(edge, distance);
            }

            return distancesBetweenEdgeNodes;
        }

        private void ComputeEdgeRoutePoints(
            TEdge edge, 
            double offset, 
            CancellationToken cancellationToken, 
            IDictionary<TVertex, int> edgesPerVertex, 
            IDictionary<TVertex, int> edgesDrawnPerVertex)
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

            if (edge.Source.ID < edge.Target.ID)
            {
                ComputeFirstCase(edge, offset, edgesPerVertex, edgesDrawnPerVertex, 1);
            }
            else
            {
                ComputeFirstCase(edge, offset, edgesPerVertex, edgesDrawnPerVertex, -1);
            }
        }

        private void ComputeFirstCase(
            TEdge edge, 
            double offset, 
            IDictionary<TVertex, int> edgesPerVertex, 
            IDictionary<TVertex, int> edgesDrawnPerVertex, 
            int direction)
        {
            var sourcePoint = VertexPositions[edge.Source];

            var targetPoint = VertexPositions[edge.Target];

            if (sourcePoint == targetPoint)
            {
                return;
            }

            var sourceVertexWidth = VertexSizes[edge.Source].Width;
            var targetVertexWidth = VertexSizes[edge.Target].Width;

            var x = sourcePoint.X
                    + ((sourceVertexWidth / edgesPerVertex[edge.Source])
                       * (edgesPerVertex[edge.Source] + direction * edgesDrawnPerVertex[edge.Source]));
            var y = sourcePoint.Y + offset;

            var adjustedSourcePoint = new Point(x - 100.0, y);

            var tempList = new List<Point> { adjustedSourcePoint };

            x = targetPoint.X
                + (direction * ((targetVertexWidth / edgesPerVertex[edge.Target]) * edgesDrawnPerVertex[edge.Target]));
            y = targetPoint.Y + offset;

            var adjustedTargetPoint = new Point(x, y);

            x = sourcePoint.X
                + ((sourceVertexWidth / edgesPerVertex[edge.Source])
                   * (edgesPerVertex[edge.Source] + direction * edgesDrawnPerVertex[edge.Source]));
            y = sourcePoint.Y + offset;

            tempList.Add(new Point(x, y));

            x = targetPoint.X
                + (direction * ((targetVertexWidth / edgesPerVertex[edge.Target]) * edgesDrawnPerVertex[edge.Target]));
            y = sourcePoint.Y + offset;

            tempList.Add(new Point(x, y));

            tempList.Add(adjustedTargetPoint);

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