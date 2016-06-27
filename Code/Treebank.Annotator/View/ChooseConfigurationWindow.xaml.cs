namespace Treebank.Annotator.View
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Mappers;

    public class Pair
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public partial class ChooseConfigurationWindow : Window
    {
        private readonly IAppConfigMapper appConfigMapper;

        public ChooseConfigurationWindow(IAppConfigMapper appConfigMapper, string windowTitle = "Choose Configuration")
        {
            InitializeComponent();
            Title = windowTitle;
            this.appConfigMapper = appConfigMapper;
            ConfigurationFiles = LoadConfigFiles();
            configurationFiles.ItemsSource = ConfigurationFiles;
            configurationFiles.SelectedItem = ConfigurationFiles.FirstOrDefault();
        }

        public Pair SelectedConfigFile
        {
            get { return configurationFiles.SelectedItem as Pair; }
        }

        public ObservableCollection<Pair> ConfigurationFiles { get; set; }

        private ObservableCollection<Pair> LoadConfigFiles()
        {
            var configFilesDirectoryPath = ConfigurationManager.AppSettings["configurationFilesDirectoryPath"];

            var filePaths = Directory.GetFiles(configFilesDirectoryPath, "*.xml");

            var result = new ObservableCollection<Pair>();

            foreach (var filePath in filePaths)
            {
                var isValid = true;

                try
                {
                    var appconfig = appConfigMapper.Map(filePath);
                    if (appconfig == null)
                    {
                        throw new ArgumentNullException();
                    }
                }
                catch (Exception)
                {
                    isValid = false;
                }

                if (isValid)
                {
                    result.Add(new Pair
                    {
                        FileName = Path.GetFileName(filePath),
                        FilePath = filePath
                    });
                }
                else
                {
                    MessageBox.Show(string.Format("The configuration file {0} is not valid. Please remove/correct it.",
                        filePath));
                }
            }

            return result;
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}