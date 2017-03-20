namespace Treebank.Annotator.AutofacModules
{
    using Autofac;
    using Prism.Events;
    using View.Services;
    using ViewModels;
    using Mappers;
    using Persistence;

    public class AnnotatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<SaveDialogService>().As<ISaveDialogService>();
            builder.RegisterType<OpenFileDialogService>().As<IOpenFileDialogService>();
            builder.RegisterType<ShowMessageBoxInfo>().As<IShowInfoMessage>();
            builder.RegisterType<SentenceLoader>().As<ISentenceLoader>();
            builder.RegisterType<AppConfigMapper>().As<IAppConfigMapper>().PropertiesAutowired();
            builder.RegisterType<SentenceEditorViewModel>().AsSelf();
            builder.RegisterType<AddWordViewModel>().AsSelf();
            builder.RegisterType<WordReorderingViewModel>().AsSelf();
            builder.RegisterType<ElementAttributeEditorViewModel>().AsSelf();
            builder.RegisterType<CompareSentencesViewModel>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
        }
    }
}