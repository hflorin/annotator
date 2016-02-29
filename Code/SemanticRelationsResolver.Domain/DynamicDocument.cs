namespace SemanticRelationsResolver.Domain
{
    using System;

    public abstract class DynamicDocument : Document
    {
        protected DynamicDocument(dynamic content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content", "Must provide document content.");
            }

            DocumentContent = content;

            Initialize();
        }

        public dynamic DocumentContent { get; set; }

        protected abstract void Initialize();
    }
}