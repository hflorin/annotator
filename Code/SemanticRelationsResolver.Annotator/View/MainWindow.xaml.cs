namespace SemanticRelationsResolver.Annotator.View
{
    using System.Windows;

    using Microsoft.Win32;

    public partial class MainWindow : Window
    {
        private const string AllowedTreebankFileFormatsFilter = "XML files (*.xml)|*.xml";

        private const string AllFilesFilter = "All files (*.*)|*.*";

        private static string currentTreebankFilepath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
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
            currentTreebankFilepath = GetSaveFileLocation();

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }
        }

        private void TreebankFromExistingClick(object sender, RoutedEventArgs e)
        {
            currentTreebankFilepath = GetFileLocation();

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                currentTreebankFilepath = GetSaveFileLocation();
            }

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }
        }

        private void SaveAsClick(object sender, RoutedEventArgs e)
        {
            currentTreebankFilepath = GetSaveFileLocationAllFiles();

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            currentTreebankFilepath = GetFileLocation();

            if (string.IsNullOrWhiteSpace(currentTreebankFilepath))
            {
                return;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            // todo:check if there are any unsaved changes, show popup to allow the user to decide, handle his response and then exit the app
            Application.Current.Shutdown();
        }
    }
}