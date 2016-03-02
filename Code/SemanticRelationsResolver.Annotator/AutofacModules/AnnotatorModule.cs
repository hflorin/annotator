namespace SemanticRelationsResolver.Annotator.AutofacModules
{
    using Autofac;
    using Loaders;
    using Mappers;

    using SemanticRelationsResolver.Annotator.View.Services;

    using ViewModels;

    public class AnnotatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DynamicXmlLoader>().As<IResourceLoader>();
            builder.RegisterType<TreebankMapper>().As<IDocumentMapper>().PropertiesAutowired();
            builder.RegisterType<SaveDialogService>().As<ISaveDialogService>();
            builder.RegisterType<OpenFileDialogService>().As<IOpenFileDialogService>();
            builder.RegisterType<MainViewModel>().AsSelf();
        }
    }
}