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

            var offset = -30;
            for (var i = 0; i < edgesSortedByDistanceBetweenVertices.Count; i++)
            {
                ComputeEdgeRoutePoints(
                    edgesSortedByDistanceBetweenVertices[i].Key,
                    offset,
                    cancellationToken,
                    numberOfEdgesPerVertex,
                    numberOfEdgesDrawnPerVertex);

                if ((i+1 < edgesSortedByDistanceBetweenVertices.Count) && edgesSortedByDistanceBetweenVertices[i].Value.CompareTo(edgesSortedByDistanceBetweenVertices[i+1].Value) != 0)
                {
                    offset -= 15;
                }
            }
            //foreach (var item in edgesSortedByDistanceBetweenVertices)
            //{
            //    ComputeEdgeRoutePoints(
            //        item.Key,
            //        offset,
            //        cancellationToken,
            //        numberOfEdgesPerVertex,
            //        numberOfEdgesDrawnPerVertex);
            //    offset -= 10;
            //}
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

            ComputeSecondCase(edge, offset);

            //if (edge.Source.ID < edge.Target.ID)
            //{
            //    ComputeFirstCase(edge, offset, edgesPerVertex, edgesDrawnPerVertex);
            //}
            //else
            //{
            //    ComputeSecondCase(edge, offset, edgesPerVertex, edgesDrawnPerVertex);
            //}
        }

        private void ComputeFirstCase(
            TEdge edge,
            double offset,
            IDictionary<TVertex, int> edgesPerVertex,
            IDictionary<TVertex, int> edgesDrawnPerVertex)
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

            x = sourcePoint.X
                + (sourceVertexWidth/edgesPerVertex[edge.Source]
                   *edgesPerVertex[edge.Source] + edgesDrawnPerVertex[edge.Source]);
            y = sourcePoint.Y + offset;

            tempList.Add(new Point(x, y));

            x = targetPoint.X
                + targetVertexWidth/edgesPerVertex[edge.Target]*edgesDrawnPerVertex[edge.Target];
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

            edgesDrawnPerVertex[edge.Source]++;
            edgesDrawnPerVertex[edge.Target]++;
        }

        private void ComputeSecondCase(
            TEdge edge,
            double offset
            //IDictionary<TVertex, int> edgesPerVertex,
            /*IDictionary<TVertex, int> edgesDrawnPerVertex*/)
        {
            var sourcePoint = VertexPositions[edge.Source];

            var targetPoint = VertexPositions[edge.Target];

            if (sourcePoint == targetPoint)
            {
                return;
            }

            var sourceVertexWidth = VertexSizes[edge.Source].Width;
            var targetVertexWidth = VertexSizes[edge.Target].Width;

            //var x = sourcePoint.X
            //        + sourceVertexWidth/edgesPerVertex[edge.Source]
            //        *(edgesPerVertex[edge.Source] + edgesDrawnPerVertex[edge.Source]);
            //var y = sourcePoint.Y + offset;

            var x = sourcePoint.X;
            var y = sourcePoint.Y;

            var tempList = new List<Point> { new Point(x, y) };

            //x = sourcePoint.X
            //    + (sourceVertexWidth/edgesPerVertex[edge.Source]
            //       *edgesPerVertex[edge.Source] + edgesDrawnPerVertex[edge.Source]);
            x = sourcePoint.X + sourceVertexWidth / 2;
            y = sourcePoint.Y + offset;
            tempList.Add(new Point(x, y));



            //x = targetPoint.X
              //  + targetVertexWidth/edgesPerVertex[edge.Target]*edgesDrawnPerVertex[edge.Target];
            x = targetPoint.X + targetVertexWidth/2;
            y = sourcePoint.Y + offset;
            tempList.Add(new Point(x, y));



            //x = targetPoint.X + targetVertexWidth / edgesPerVertex[edge.Target] * edgesDrawnPerVertex[edge.Target];
            //y = targetPoint.Y + offset;
            x = targetPoint.X;
            y = targetPoint.Y;

            tempList.Add(new Point(x+100, y));

            if (EdgeRoutes.ContainsKey(edge))
            {
                EdgeRoutes[edge] = tempList.Count > 2 ? tempList.ToArray() : null;
            }
            else
            {
                EdgeRoutes.Add(edge, tempList.Count > 2 ? tempList.ToArray() : null);
            }

           // edgesDrawnPerVertex[edge.Source]++;
           // edgesDrawnPerVertex[edge.Target]++;
        }
    }
}