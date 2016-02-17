namespace SemanticRelationsResolver.Domain
{
    using System.Dynamic;

    public class DynamicDocument : Document
    {
        public DynamicDocument(ExpandoObject content)
        {
            DocumentCotent = content;
        }

        public ExpandoObject DocumentCotent { get; private set; }
    }
}