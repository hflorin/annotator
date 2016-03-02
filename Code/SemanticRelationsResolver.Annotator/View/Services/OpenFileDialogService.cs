namespace SemanticRelationsResolver.Annotator.View.Services
{
    using Microsoft.Win32;

    public class OpenFileDialogService : IOpenFileDialogService
    {
        public string GetFileLocation(string fileFilters)
        {
            var openFileDialog = new OpenFileDialog { Filter = fileFilters };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }
    }
}