namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using Wrapper;
    using Wrapper.Base;

    public class ElementAttributeEditorViewModel : Observable
    {
        public ChangeTrackingCollection<AttributeWrapper> Attributes { get; set; }
    }
}