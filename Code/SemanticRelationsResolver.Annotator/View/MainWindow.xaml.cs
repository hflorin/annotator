namespace SemanticRelationsResolver.Annotator.View
{
    using System.ComponentModel;
    using System.Windows;

    using Microsoft.Win32;

    using SemanticRelationsResolver.Annotator.ViewModels;

    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        private const string AllowedTreebankFileFormatsFilter = "XML files (*.xml)|*.xml";

        private const string AllFilesFilter = "All files (*.*)|*.*";

        private static string _currentTreebankFilepath = string.Empty;

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

        private static string GetSaveFileLocation()
        {
            var saveFileDialog = new SaveFileDialog { Filter = AllowedTreebankFileFormatsFilter };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : string.Empty;
        }

        private static string GetSaveFileLocationAllFiles()
        {
            var saveFileDialog = new SaveFileDialog { Filter = AllFilesFilter };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : string.Empty;
        }

        private static string GetFileLocation()
        {
            var openFileDialog = new OpenFileDialog { Filter = AllowedTreebankFileFormatsFilter };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }



        private void TreebankClick(object sender, RoutedEventArgs e)
        {
            _currentTreebankFilepath = GetSaveFileLocation();

            if (string.IsNullOrWhiteSpace(_currentTreebankFilepath))
            {
                return;
            }
        }

        private void TreebankFromExistingClick(object sender, RoutedEventArgs e)
        {
            _currentTreebankFilepath = GetFileLocation();

            if (string.IsNullOrWhiteSpace(_currentTreebankFilepath))
            {
                return;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentTreebankFilepath))
            {
                _currentTreebankFilepath = GetSaveFileLocation();
            }

            if (string.IsNullOrWhiteSpace(_currentTreebankFilepath))
            {
                return;
            }
        }

        private void SaveAsClick(object sender, RoutedEventArgs e)
        {
            _currentTreebankFilepath = GetSaveFileLocationAllFiles();

            if (string.IsNullOrWhiteSpace(_currentTreebankFilepath))
            {
                return;
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            _currentTreebankFilepath = GetFileLocation();

            if (string.IsNullOrWhiteSpace(_currentTreebankFilepath))
            {
                return;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            // todo:check if there are any unsaved changes, show popup to allow the user to decide, handle his response and then exit the app
            Close();
        }
    }
}