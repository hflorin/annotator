namespace Treebank.Annotator
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Autofac;
    using NLog;
    using Prism.Events;
    using Startup;
    using View;
    using ViewModels;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += ExceptionHandler;

            base.OnStartup(e);

            // set tooltip to show indefinetly
            ToolTipService.ShowDurationProperty.OverrideMetadata(
                typeof(DependencyObject),
                new FrameworkPropertyMetadata(int.MaxValue));

            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(10));

            var bootstrapper = new Bootstrapper();

            var container = bootstrapper.Boostrap();

            var viewModel = container.Resolve<MainViewModel>();

            MainWindow = new MainWindow(viewModel, container.Resolve<IEventAggregator>());

            MainWindow.Show();
        }

        private void ExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = LogManager.GetCurrentClassLogger(typeof(App));
            logger.Error(e.ExceptionObject);

            MessageBox.Show("An unexpectected error occured! See logs for more details.");
            var mainViewModel = MainWindow.DataContext as MainViewModel;

            if (mainViewModel != null)
            {
                mainViewModel.OnClosing(null);
            }
        }
    }
}