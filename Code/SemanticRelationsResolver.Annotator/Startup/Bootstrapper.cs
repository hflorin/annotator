namespace SemanticRelationsResolver.Annotator.Startup
{
    using Autofac;
    using AutofacModules;

    public class Bootstrapper
    {
        public IContainer Boostrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AnnotatorModule>();

            return builder.Build();
        }
    }
}