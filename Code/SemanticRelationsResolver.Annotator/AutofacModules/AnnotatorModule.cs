namespace SemanticRelationsResolver.Annotator.AutofacModules
{
    using Autofac;
    using Loaders;
    using Mappers;
    using ViewModels;

    public class AnnotatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DynamicXmlLoader>().As<IResourceLoader>();
            builder.RegisterType<XmlDocumentMapper>().As<IDocumentMapper>().PropertiesAutowired();
            builder.RegisterType<MainViewModel>().AsSelf();
        }
    }
}