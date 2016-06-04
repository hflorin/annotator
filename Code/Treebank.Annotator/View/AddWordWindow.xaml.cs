namespace Treebank.Annotator.View
{
    using System;
    using System.Windows;
    using Treebank.Annotator.ViewModels;

    public partial class AddWordWindow : Window
    {
        private readonly AddWordViewModel viewModel;

        public AddWordWindow(AddWordViewModel viewModel)
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