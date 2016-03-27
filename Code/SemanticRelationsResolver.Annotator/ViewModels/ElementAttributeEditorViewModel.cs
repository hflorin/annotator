namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using Domain;
    using Wrapper;
    using Wrapper.Base;

    public class ElementAttributeEditorViewModel : Observable
    {
        private ChangeTrackingCollection<AttributeWrapper> attributes;
        public ChangeTrackingCollection<AttributeWrapper> Attributes { get { return attributes; } set { attributes = value; } }
    }
}