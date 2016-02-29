namespace SemanticRelationsResolver.Annotator.Commands
{
    using System.Windows.Input;

    public static class ApplicationCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
            "Exit",
            "Exit",
            typeof(ApplicationCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
            );
    }
}