namespace Treebank.Annotator.View
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewModels;

    public partial class ElementAttributeEditorView : UserControl
    {
        private readonly ElementAttributeEditorViewModel viewModel;

        public ElementAttributeEditorView()
        {
            InitializeComponent();
        }

        public ElementAttributeEditorView(ElementAttributeEditorViewModel viewModel) : this()
        {
            this.viewModel = viewModel;

            DataContext = this.viewModel;
        }

        private void DataGrid_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var model = DataContext as ElementAttributeEditorViewModel;
            if (model != null)
            {
                model.InvalidateCommands();
            }
        }
    }
}