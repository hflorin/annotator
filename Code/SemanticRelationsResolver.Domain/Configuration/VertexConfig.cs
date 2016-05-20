namespace SemanticRelationsResolver.Domain.Configuration
{
    public class VertexConfig
    {
        public string Entity { get; set; }
        public string LabelAttributeName { get; set; }
        public string FromAttributeName { get; set; }
        public string ToAttributeName { get; set; }
        public string RootVertex { get; set; }
    }
}