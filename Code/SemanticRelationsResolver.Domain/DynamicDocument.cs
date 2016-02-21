namespace SemanticRelationsResolver.Domain
{
    using System;
    using System.Dynamic;

    public abstract class DynamicDocument : Document
    {
        protected DynamicDocument(ExpandoObject content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content", "Must provide document content.");
            }

            DocumentCotent = content;

            Initialize();
        }

        public ExpandoObject DocumentCotent { get; set; }

        protected abstract void Initialize();
    }
}