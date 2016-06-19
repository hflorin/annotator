namespace Treebank.Annotator.View
{
    using System;
    using System.Windows;
    using ViewModels;

    /// <summary>
    ///     Interaction logic for CompareSentencesWindow.xaml
    /// </summary>
    public partial class CompareSentencesWindow : Window
    {
        private readonly CompareSentencesViewModel viewModel;

        public CompareSentencesWindow(CompareSentencesViewModel viewModel)
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