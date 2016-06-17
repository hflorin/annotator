namespace Treebank.Annotator.View.Services
{
    using System.Windows;

    public class ShowMessageBoxInfo : IShowInfoMessage
    {
        public MessageBoxResult ShowInfoMessage(string message, MessageBoxButton buttons)
        {
            return MessageBox.Show(message, "Info", buttons);
        }
    }
}