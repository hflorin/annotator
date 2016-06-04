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

            DynamicContent = content;

            Initialize();
        }

        public dynamic DynamicContent { get; set; }

        protected abstract void Initialize();
    }
}