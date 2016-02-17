namespace SemanticRelationsResolver.Console.App
{
    using Config;
    using Mappers;

    public class SemanticRelationsResolverApp : ISemanticRelationsResolverApp
    {
        public IDocumentMapper DocumentMapper { get; set; }

        public ConsoleConfig Config { get; set; }

        public void Run()
        {
            var documentContent = DocumentMapper.Map(Config.InputDirectoryPath + "XML\\QuoVadisPartial.xml");
        }
    }
}
