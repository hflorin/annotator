namespace SemanticRelationsResolver.Annotator.View
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using Events;
    using GraphX.Controls;
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

            GgArea.RelayoutFinished += GgArea_RelayoutFinished;
            GgArea.GenerateGraphFinished += GgArea_GenerateGraphFinished;

            viewModel.EventAggregator.GetEvent<RelayoutGraphEvent>().Subscribe(OnRelayoutGraph);
        }

        private void OnRelayoutGraph(bool relayout)
        {
            if (relayout)
            {
                DisplayGraph();
            }
        }

        public void Dispose()
        {
            if (GgArea != null)
            {
                GgArea.Dispose();
            }
        }

        private void GgArea_GenerateGraphFinished(object sender, EventArgs e)
        {
            if (!GgArea.EdgesList.Any())
            {
                GgArea.GenerateAllEdges();
            }

            GgArea.ShowAllEdgesArrows();
            GgArea.ShowAllEdgesLabels();

            GgZoomCtrl.ZoomToFill();
            GgZoomCtrl.Mode = ZoomControlModes.Custom;
        }

        private void GgArea_RelayoutFinished(object sender, EventArgs e)
        {
            GgZoomCtrl.ZoomToFill();
            GgZoomCtrl.Mode = ZoomControlModes.Custom;
        }

        private void viewModel_SentenceGraphChanged(object sender, EventArgs e)
        {
            DisplayGraph();
        }

        private void DisplayGraph()
        {
            GgArea.GenerateGraph();
            GgArea.ShowAllEdgesArrows();
            GgArea.ShowAllEdgesLabels();
            GgArea.RelayoutGraph(true);
            if (!GgArea.EdgesList.Any())
            {
                GgArea.GenerateAllEdges();
            }
            GgZoomCtrl.ZoomToFill();
            GgZoomCtrl.Mode = ZoomControlModes.Custom;
        }
    }
}