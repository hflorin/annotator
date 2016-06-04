namespace Treebank.Annotator.View
{
    using System.Windows.Controls;
    using Treebank.Annotator.ViewModels;

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