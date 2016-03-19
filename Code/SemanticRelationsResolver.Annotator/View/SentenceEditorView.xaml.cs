namespace SemanticRelationsResolver.Annotator.View
{
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
    }
}