namespace Treebank.Annotator.View.Services
{
    using Microsoft.Win32;

    public class SaveDialogService : ISaveDialogService
    {
        public string GetSaveFileLocation(string fileFilters)
        {
            var saveFileDialog = new SaveFileDialog {Filter = fileFilters};

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : string.Empty;
        }
    }
}