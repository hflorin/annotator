namespace SemanticRelationsResolver.Annotator.View
{
    using System.Windows.Controls;
    using ViewModels;

    public partial class ElementAttributeEditorView : UserControl
    {
        private readonly ElementAttributeEditorViewModel viewModel;

        public ElementAttributeEditorView()
        {
            InitializeComponent();
        }

        public ElementAttributeEditorView(ElementAttributeEditorViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }
    }
}