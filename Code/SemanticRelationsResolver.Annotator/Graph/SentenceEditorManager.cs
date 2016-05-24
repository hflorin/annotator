namespace SemanticRelationsResolver.Annotator.Graph
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using GraphX.Controls;
    using GraphX.Controls.Models;

    public class SentenceEditorManager : IDisposable
    {
        private EdgeBlueprint edgeBlueprint;

        private SentenceGraphArea graphArea;

        private ResourceDictionary resourceDictionary;

        private ZoomControl zoomControl;

        public SentenceEditorManager(SentenceGraphArea graphArea, ZoomControl zc)
        {
            this.graphArea = graphArea;
            zoomControl = zc;
            zoomControl.MouseMove += _zoomControl_MouseMove;
            resourceDictionary = new ResourceDictionary
                                     {
                                         Source =
                                             new Uri(
                                             "/SemanticRelationsResolver.Annotator;component/Templates/SentenceEditorViewTemplates.xaml", 
                                             UriKind.RelativeOrAbsolute)
                                     };
        }

        public void Dispose()
        {
            ClearEdgeBp();
            graphArea = null;
            if (zoomControl != null)
            {
                zoomControl.MouseMove -= _zoomControl_MouseMove;
            }

            zoomControl = null;
            resourceDictionary = null;
        }

        public void CreateVirtualEdge(VertexControl source, Point mousePos)
        {
            edgeBlueprint = new EdgeBlueprint(source, mousePos, (LinearGradientBrush)resourceDictionary["EdgeBrush"]);
            graphArea.InsertCustomChildControl(0, edgeBlueprint.EdgePath);
        }

        private void _zoomControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (edgeBlueprint == null)
            {
                return;
            }

            var pos = zoomControl.TranslatePoint(e.GetPosition(zoomControl), graphArea);
            pos.Offset(2, 2);
            edgeBlueprint.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBp()
        {
            if (edgeBlueprint == null)
            {
                return;
            }

            graphArea.RemoveCustomChildControl(edgeBlueprint.EdgePath);
            edgeBlueprint.Dispose();
            edgeBlueprint = null;
        }

        public void DestroyVirtualEdge()
        {
            ClearEdgeBp();
        }
    }

    public class EdgeBlueprint : IDisposable
    {
        public EdgeBlueprint(VertexControl source, Point targetPos, Brush brush)
        {
            EdgePath = new Path { Stroke = brush, Data = new LineGeometry() };
            Source = source;
            Source.PositionChanged += Source_PositionChanged;
        }

        public VertexControl Source { get; set; }

        public Point TargetPos { get; set; }

        public Path EdgePath { get; set; }

        public void Dispose()
        {
            Source.PositionChanged -= Source_PositionChanged;
            Source = null;
        }

        private void Source_PositionChanged(object sender, VertexPositionEventArgs args)
        {
            UpdateGeometry(Source.GetCenterPosition(), TargetPos);
        }

        internal void UpdateTargetPosition(Point point)
        {
            TargetPos = point;
            UpdateGeometry(Source.GetCenterPosition(), point);
        }

        private void UpdateGeometry(Point start, Point end)
        {
            EdgePath.Data = new LineGeometry(start, end);
            (EdgePath.Data as LineGeometry).Freeze();
        }
    }
}