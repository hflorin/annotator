namespace SemanticRelationsResolver.Domain
{
    using System;
    using System.Dynamic;

    public class Treebank : DynamicDocument
    {
        public Treebank(ExpandoObject content) : base(content)
        {
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}