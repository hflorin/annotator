namespace Treebank.Annotator.View
{
    using System;
    using System.Windows;
    using ViewModels;

    public partial class AddEdgeWindow : Window
    {
        private readonly AddEdgeViewModel viewModel;

        public AddEdgeWindow(AddEdgeViewModel viewModel)
        {
            InitializeComponent();
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            this.viewModel = viewModel;
            DataContext = this.viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}