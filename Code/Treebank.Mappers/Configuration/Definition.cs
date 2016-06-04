namespace Treebank.Mappers.Configuration
{
    public class Definition
    {
        public string Name { get; set; }

        public VertexConfig Vertex { get; set; }
        public EdgeConfig Edge { get; set; }
    }
}