namespace SemanticRelationsResolver.Annotator.AutofacModules
{
    using Autofac;
    using Loaders;
    using Mappers;
    using Prism.Events;
    using View.Services;
    using ViewModels;

    public class AnnotatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<DynamicXmlLoader>().As<IResourceLoader>();
            builder.RegisterType<DocumentMapper>().As<IDocumentMapper>().PropertiesAutowired();
            builder.RegisterType<SaveDialogService>().As<ISaveDialogService>();
            builder.RegisterType<OpenFileDialogService>().As<IOpenFileDialogService>();
            builder.RegisterType<MainViewModel>().AsSelf();
        }
    }
}