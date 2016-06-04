namespace Treebank.Annotator.View
{
    using System;
    using System.Windows;
    using ViewModels;

    public partial class AddAttributeWindow : Window
    {
        private readonly AddAttributeViewModel viewModel;

        public AddAttributeWindow(AddAttributeViewModel viewModel)
        {
            InitializeComponent();
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            this.viewModel = viewModel;
            DataContext = this.viewModel;
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}