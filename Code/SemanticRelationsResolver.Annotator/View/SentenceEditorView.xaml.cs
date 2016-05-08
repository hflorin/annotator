namespace SemanticRelationsResolver.Annotator.View
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Events;
    using Graph;
    using GraphX.Controls;
    using GraphX.Controls.Models;
    using GraphX.PCL.Logic.Helpers;
    using Prism.Events;
    using ViewModels;

    public partial class SentenceEditorView : UserControl, IDisposable
    {
        private readonly IEventAggregator eventAggregator;
        private readonly SentenceEditorViewModel viewModel;

        public SentenceEditorView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");
            this.eventAggregator = eventAggregator;
        }

        public SentenceEditorView(SentenceEditorViewModel sentenceEditorViewModel, IEventAggregator eventAggregator)
        {
            InitializeComponent();

            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");
            this.eventAggregator = eventAggregator;

            viewModel = sentenceEditorViewModel;
            DataContext = viewModel;

            viewModel.Initialize();
            GgZoomCtrl.MouseLeftButtonUp += GgZoomCtrl_MouseLeftButtonUp;
            GgArea.VertexSelected += GgArea_VertexSelected;
            GgArea.RelayoutFinished += GgArea_RelayoutFinished;
            GgArea.GenerateGraphFinished += GgArea_GenerateGraphFinished;

            viewModel.EventAggregator.GetEvent<RelayoutGraphEvent>().Subscribe(OnRelayoutGraph);
            viewModel.EventAggregator.GetEvent<SetSentenceEditModeEvent>().Subscribe(OnSetSentenceEditMode);
        }

        private void OnSetSentenceEditMode(SetSenteceGraphOperationModeRequest setSenteceGraphOperationModeRequest)
        {
            if (butDelete.IsChecked == true && setSenteceGraphOperationModeRequest.Mode == SenteceGraphOperationMode.Delete)
            {
                butEdit.IsChecked = false;
                butSelect.IsChecked = false;
                GgZoomCtrl.Cursor = Cursors.Help;
                viewModel.SenteceGraphOperationMode = SenteceGraphOperationMode.Delete;
            //    ClearEditMode();
              //  ClearSelectMode();
                return;
            }
            if (butEdit.IsChecked == true && setSenteceGraphOperationModeRequest.Mode == SenteceGraphOperationMode.Edit)
            {
                butDelete.IsChecked = false;
                butSelect.IsChecked = false;
                GgZoomCtrl.Cursor = Cursors.Pen;
                viewModel.SenteceGraphOperationMode = SenteceGraphOperationMode.Edit;
                //ClearSelectMode();
                return;
            }
            if (butSelect.IsChecked == true && setSenteceGraphOperationModeRequest.Mode == SenteceGraphOperationMode.Select)
            {
                butEdit.IsChecked = false;
                butDelete.IsChecked = false;
                GgZoomCtrl.Cursor = Cursors.Hand;
                viewModel.SenteceGraphOperationMode = SenteceGraphOperationMode.Select;
                //ClearEditMode();
                GgArea.SetVerticesDrag(true, true);
                return;
            }
        }

        public void Dispose()
        {
            if (GgArea != null)
            {
                GgArea.Dispose();
            }
        }

        private void GgZoomCtrl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GgArea.VertexList.ForEach(pair =>
            {
                if (DragBehaviour.GetIsTagged(pair.Value))
                {
                    HighlightBehaviour.SetHighlighted(pair.Value, false);
                    DragBehaviour.SetIsTagged(pair.Value, false);
                }
            });
        }

        private void GgArea_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                if (viewModel.SenteceGraphOperationMode == SenteceGraphOperationMode.Edit)
                  //  CreateEdgeControl(args.VertexControl);
               // else if (viewModel.SenteceGraphOperationMode == SenteceGraphOperationMode.Delete)
                   // SafeRemoveVertex(args.VertexControl);
               // else if (viewModel.SenteceGraphOperationMode == SenteceGraphOperationMode.Select && args.Modifiers == ModifierKeys.Control)
                    SelectVertex(args.VertexControl);
            }
        }

        private void SelectVertex(VertexControl vertexControl)
        {
            var vertex = vertexControl.Vertex as WordVertex;
            if (vertex != null)
            {
                eventAggregator.GetEvent<ChangeAttributesEditorViewModel>()
                    .Publish(new ElementAttributeEditorViewModel { Attributes = vertex.WordWrapper.Attributes });
            }

            if (DragBehaviour.GetIsTagged(vertexControl))
            {
                HighlightBehaviour.SetHighlighted(vertexControl, false);
                DragBehaviour.SetIsTagged(vertexControl, false);
            }
            else
            {
                HighlightBehaviour.SetHighlighted(vertexControl, true);
                DragBehaviour.SetIsTagged(vertexControl, true);
            }
        }


        private void OnRelayoutGraph(bool relayout)
        {
            if (relayout)
            {
                DisplayGraph();
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