namespace SemanticRelationsResolver.Domain
{
    using System.Dynamic;

    public class DynamicDocument : Document
    {
        public ExpandoObject DocumentCotent { get; private set; }

        public DynamicDocument(ExpandoObject content)
        {
            DocumentCotent = content;
        }

    }
}
