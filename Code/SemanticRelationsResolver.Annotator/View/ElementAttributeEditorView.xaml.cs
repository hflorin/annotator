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

        public ElementAttributeEditorView(ElementAttributeEditorViewModel viewModel):this()
        {
            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }
    }
}