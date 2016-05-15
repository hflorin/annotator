namespace SemanticRelationsResolver.Annotator.View
{
    using System;
    using System.Windows;
    using ViewModels;

    public partial class WordReorderingWindow : Window
    {
        public WordReorderingWindow(WordReorderingViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            InitializeComponent();

            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}