namespace Treebank.Annotator.Graph
{
    using System.Windows;
    using GraphX.Controls;
    using QuickGraph;

    public class SentenceGraphArea : GraphArea<WordVertex, WordEdge, BidirectionalGraph<WordVertex, WordEdge>>
    {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                "GraphLogicCoreData",
                typeof(SentenceGxLogicCore),
                typeof(SentenceGraphArea),
                new PropertyMetadata(default(SentenceGxLogicCore), OnItemsPropertyChanged));

        public SentenceGxLogicCore GraphLogicCoreData
        {
            get { return (SentenceGxLogicCore) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private static void OnItemsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var graph = (SentenceGraphArea) obj;

           graph.RelayoutGraph(true);
           // graph.GenerateGraph();
        }
    }
}