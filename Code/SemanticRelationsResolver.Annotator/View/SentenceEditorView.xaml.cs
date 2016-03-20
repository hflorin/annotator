namespace SemanticRelationsResolver.Annotator.View
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    public partial class SentenceEditorView : UserControl,IDisposable
    {
        public SentenceEditorView(SentenceEditorViewModel sentenceEditorViewModel)
        {
            InitializeComponent();

            var viewModel = sentenceEditorViewModel;
            DataContext = viewModel;


            viewModel.Initialize();
        }

        void viewModel_SentenceGraphChanged(object sender, EventArgs e)
        {
            GgArea.GenerateGraph();
            GgArea.RelayoutGraph(true);
        }

        public void Dispose()
        {
            if (GgArea != null)
            {
                GgArea.Dispose();
            }
        }
    }
}