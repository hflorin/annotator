namespace Treebank.Annotator.View.Services
{
    using System.Windows.Forms;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

    public class OpenFileDialogService : IOpenFileDialogService
    {
        public string GetFileLocation(string fileFilters)
        {
            var openFileDialog = new OpenFileDialog {Filter = fileFilters};

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }

        public string GetFolderLocation()
        {
            var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}