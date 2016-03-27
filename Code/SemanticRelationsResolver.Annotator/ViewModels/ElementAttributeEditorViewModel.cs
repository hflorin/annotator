namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using Domain;
    using Wrapper;

    public class ElementAttributeEditorViewModel : Observable
    {
        private ElementWrapper<Element> element;
        public ElementWrapper<Element> Element { get { return element; } set { element = value; } }
    }
}