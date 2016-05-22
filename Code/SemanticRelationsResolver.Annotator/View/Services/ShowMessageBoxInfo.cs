namespace SemanticRelationsResolver.Annotator.View.Services
{
    using System.Windows;

    public class ShowMessageBoxInfo : IShowInfoMessage
    {
        public MessageBoxResult ShowInfoMessage(string message)
        {
            return MessageBox.Show(message, "Info", MessageBoxButton.OK);
        }
    }
}