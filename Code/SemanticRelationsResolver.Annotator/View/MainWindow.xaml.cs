namespace SemanticRelationsResolver.Annotator.View
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Events;
    using Prism.Events;
    using ViewModels;
    using Wrapper;

    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;
        private readonly IEventAggregator eventAggregator;

        public MainWindow(MainViewModel viewModel, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            this.eventAggregator = eventAggregator;
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

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            eventAggregator.GetEvent<CheckIsTreeOnSentenceEvent>().Publish(e.Row.DataContext as SentenceWrapper);
        }
    }
}