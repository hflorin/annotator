namespace Treebank.Annotator.View
{
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.IO;
    using System.Windows;

    public class Pair
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public partial class ChooseConfigurationWindow : Window
    {
        public ChooseConfigurationWindow(string windowTitle = "Choose Configuration")
        {
            InitializeComponent();
            Title = windowTitle;
            ConfigurationFiles = LoadConfigFiles();
            configurationFiles.ItemsSource = ConfigurationFiles;
        }

        public Pair SelectedConfigFile { get { return configurationFiles.SelectedItem as Pair; } }

        public ObservableCollection<Pair> ConfigurationFiles { get; set; }

        private ObservableCollection<Pair> LoadConfigFiles()
        {
            var configFilesDirectoryPath = ConfigurationManager.AppSettings["configurationFilesDirectoryPath"];

            var filePaths = Directory.GetFiles(configFilesDirectoryPath);

            var result = new ObservableCollection<Pair>();

            foreach (var filePath in filePaths)
            {
                result.Add(new Pair
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath
                });
            }

            return result;
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}