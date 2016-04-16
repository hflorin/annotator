namespace SemanticRelationsResolver.Events
{
    using Prism.Events;

    using SemanticRelationsResolver.Annotator.ViewModels;

    public class ChangeAttributesEditorViewModel : PubSubEvent<ElementAttributeEditorViewModel>
    {
    }
}