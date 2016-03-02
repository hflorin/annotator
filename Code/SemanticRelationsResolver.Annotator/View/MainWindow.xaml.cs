namespace SemanticRelationsResolver.Annotator.View
{
    using System.ComponentModel;
    using System.Windows;
    using ViewModels;

    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = this.viewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            viewModel.OnClosing(e);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}