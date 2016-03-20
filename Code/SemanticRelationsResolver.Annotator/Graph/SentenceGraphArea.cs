namespace SemanticRelationsResolver.Annotator.Graph
{
    using System.Windows;
    using GraphX.Controls;
    using QuickGraph;

    public class SentenceGraphArea : GraphArea<WordVertex, WordEdge, BidirectionalGraph<WordVertex, WordEdge>>
    {
        public SentenceGxLogicCore GraphLogicCoreData
        {
            get { return (SentenceGxLogicCore)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
            "GraphLogicCoreData",
                typeof(SentenceGxLogicCore),
                typeof(SentenceGraphArea),
                new PropertyMetadata(default(SentenceGxLogicCore), OnItemsPropertyChanged));

        private static void OnItemsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var graph = (SentenceGraphArea)obj;

            // Dragging of vertices allowed
            graph.SetVerticesDrag(true);
            // Generate graph including edges
            graph.GenerateGraph();
        }
    }
}