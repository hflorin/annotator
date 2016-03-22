namespace SemanticRelationsResolver.Annotator.View
{
    using System;
    using System.Windows.Controls;
    using ViewModels;

    public partial class SentenceEditorView : UserControl, IDisposable
    {
        public SentenceEditorView()
        {
            InitializeComponent();
        }

        public SentenceEditorView(SentenceEditorViewModel sentenceEditorViewModel)
        {
            InitializeComponent();

            var viewModel = sentenceEditorViewModel;
            DataContext = viewModel;

            viewModel.Initialize();
        }

        public void Dispose()
        {
            if (GgArea != null)
            {
                GgArea.Dispose();
            }
        }

        private void viewModel_SentenceGraphChanged(object sender, EventArgs e)
        {
            GgArea.GenerateGraph();
            GgArea.RelayoutGraph(true);
        }
    }
}