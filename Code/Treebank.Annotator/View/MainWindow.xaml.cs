namespace Treebank.Annotator.View
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Events;
    using Prism.Events;
    using ViewModels;
    using Wrapper;
    using Xceed.Wpf.AvalonDock;

    public partial class MainWindow : Window
    {
        private readonly IEventAggregator eventAggregator;
        private readonly MainViewModel viewModel;

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

        private void ContentDockingManager_OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            e.Document.CanClose = false;

            var documentModel = e.Document.Content as SentenceEditorView;
            if (documentModel != null)
            {
                Dispatcher.BeginInvoke(new Action(() => viewModel.SentenceEditViews.Remove(documentModel)),
                    DispatcherPriority.Background);
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.EditSentenceCommand.Execute(null);
        }
    }
}