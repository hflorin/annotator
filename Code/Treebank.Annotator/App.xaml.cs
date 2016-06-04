namespace Treebank.Annotator
{
    using System.Windows;
    using Autofac;
    using Prism.Events;
    using Treebank.Annotator.Startup;
    using Treebank.Annotator.ViewModels;
    using View;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstrapper = new Bootstrapper();

            var container = bootstrapper.Boostrap();

            var viewModel = container.Resolve<MainViewModel>();

            MainWindow = new MainWindow(viewModel, container.Resolve<IEventAggregator>());

            MainWindow.Show();
        }
    }
}