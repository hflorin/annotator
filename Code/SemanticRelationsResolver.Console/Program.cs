namespace SemanticRelationsResolver.Console
{
    using App;
    using Autofac;
    using AutofacModules;

    internal class Program
    {
        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<SemanticRelationsResolverModule>();

            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                var app = scope.Resolve<ISemanticRelationsResolverApp>();
                app.Run();
            }
        }
    }
}