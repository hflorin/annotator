namespace Treebank.Annotator.Startup
{
    using Autofac;
    using Treebank.Annotator.AutofacModules;

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