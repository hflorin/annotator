namespace SemanticRelationsResolver.Annotator
{
    using System.Windows;
    using Autofac;
    using Startup;
    using View;
    using ViewModels;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstrapper = new Bootstrapper();

            var container = bootstrapper.Boostrap();

            var viewModel = container.Resolve<MainViewModel>();

            MainWindow = new MainWindow(viewModel);

            MainWindow.Show();
        }
    }
}