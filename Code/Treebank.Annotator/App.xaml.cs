namespace Treebank.Annotator
{
    using System.Windows;
    using System.Windows.Controls;

    using Autofac;

    using Prism.Events;

    using Treebank.Annotator.Startup;
    using Treebank.Annotator.View;
    using Treebank.Annotator.ViewModels;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // set tooltip to show indefinetly
            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject), 
                new FrameworkPropertyMetadata(int.MaxValue));

            var bootstrapper = new Bootstrapper();

            var container = bootstrapper.Boostrap();

            var viewModel = container.Resolve<MainViewModel>();

            MainWindow = new MainWindow(viewModel, container.Resolve<IEventAggregator>());

            MainWindow.Show();
        }
    }
}