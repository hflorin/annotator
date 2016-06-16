namespace Treebank.Annotator.View
{
    using System.Windows;

    public partial class InputDialog : Window
    {
        public InputDialog(string windowTitle = "Input")
        {
            InitializeComponent();
            Title = windowTitle;
        }

        public string Value
        {
            get { return tb_input.Text; }
            set { tb_input.Text = value; }
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}