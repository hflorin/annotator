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

            var distancesBetweenEdgeNodes = new Dictionary<TEdge, double>();

            //1. compute distance between edge nodes
            foreach (var edge in Graph.Edges)
            {
                var fromPoint = VertexPositions[edge.Source];
                var toPoint = VertexPositions[edge.Target];

                var distance = ((fromPoint.X - toPoint.X) * (fromPoint.X - toPoint.X))
                               + ((fromPoint.X - toPoint.X) * (fromPoint.X - toPoint.X));

                distancesBetweenEdgeNodes.Add(edge, distance);
            }

            //2.sort edges according to this distance

            var edgesSortedByDistanceBetweenVertices = distancesBetweenEdgeNodes.ToList();
            edgesSortedByDistanceBetweenVertices.Sort((left, right) => left.Value.CompareTo(right.Value));
            var offset = -10;

            foreach (var item in edgesSortedByDistanceBetweenVertices)
            {
                EdgeRoutingTest(item.Key, offset, cancellationToken);
                offset -= 10;
            }
        }

        private void EdgeRoutingTest(TEdge edge, double offset, CancellationToken cancellationToken)
        {
            if (edge.Source.ID == -1 || edge.Target.ID == -1)
            {
                throw new GX_InvalidDataException(
                    "SimpleEdgeRouting() -> You must assign unique ID for each vertex to use SimpleER algo!");
            }

            if (edge.Source.ID == edge.Target.ID || !VertexPositions.ContainsKey(edge.Target))
            {
                return;
            }

            var startPoint = VertexPositions[edge.Source];

            var endPoint = VertexPositions[edge.Target];

            if (startPoint == endPoint)
            {
                return;
            }

            var tempList = new List<Point>() { startPoint };

            var sourceVertexWidth = VertexSizes[edge.Source];
            var targetVertexWidth = VertexSizes[edge.Target];

            tempList.Add(new Point(startPoint.X+sourceVertexWidth.Width/2, startPoint.Y + offset));

            tempList.Add(new Point(endPoint.X + targetVertexWidth.Width/2, startPoint.Y + offset));

            tempList.Add(endPoint);

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