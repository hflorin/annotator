namespace SemanticRelationsResolver.Annotator.View
{
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    public partial class SentenceEditorView : UserControl
    {
        private readonly SentenceEditorViewModel viewModel;

        public SentenceEditorView(SentenceEditorViewModel sentenceEditorViewModel)
        {
            InitializeComponent();
            viewModel = sentenceEditorViewModel;
            DataContext = viewModel;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            GgArea.RelayoutGraph(true);
            GgArea.GenerateGraph();
        }
    }
}