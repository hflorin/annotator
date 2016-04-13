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
                GgArea.GenerateGraph();
                GgArea.RelayoutGraph(true);

                GgZoomCtrl.ZoomToFill();
                GgZoomCtrl.Mode = ZoomControlModes.Custom;
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
            GgArea.GenerateGraph();
            GgArea.RelayoutGraph(true);
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OnRelayoutGraph(true);
        }
    }
}