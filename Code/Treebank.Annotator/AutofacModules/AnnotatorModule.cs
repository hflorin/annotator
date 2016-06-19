namespace Treebank.Annotator.AutofacModules
{
    using Autofac;
    using Loaders;
    using Mappers;
    using Persistence;
    using Prism.Events;
    using View.Services;
    using ViewModels;

    public class AnnotatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<DynamicXmlLoader>().As<IResourceLoader>();
            builder.RegisterType<XmlPersister>().As<IPersister>();
            builder.RegisterType<DocumentMapperWithReader>().As<IDocumentMapper>().PropertiesAutowired();
            builder.RegisterType<SaveDialogService>().As<ISaveDialogService>();
            builder.RegisterType<OpenFileDialogService>().As<IOpenFileDialogService>();
            builder.RegisterType<ShowMessageBoxInfo>().As<IShowInfoMessage>();
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