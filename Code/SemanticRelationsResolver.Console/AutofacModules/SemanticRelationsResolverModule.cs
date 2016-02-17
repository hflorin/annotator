namespace SemanticRelationsResolver.Console.AutofacModules
{
    using App;
    using Autofac;
    using Config;
    using Loaders;
    using Mappers;

    public class SemanticRelationsResolverModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppConfig>().As<IAppConfig>();
            builder.RegisterType<DynamicXmlLoader>().As<IResourceLoader>();
            builder.RegisterType<XmlDocumentMapper>().As<IDocumentMapper>().PropertiesAutowired();
            builder.RegisterType<SemanticRelationsResolverApp>()
                .As<ISemanticRelationsResolverApp>()
                .PropertiesAutowired();
            builder.RegisterType<ConsoleConfig>().PropertiesAutowired();
        }
    }
}