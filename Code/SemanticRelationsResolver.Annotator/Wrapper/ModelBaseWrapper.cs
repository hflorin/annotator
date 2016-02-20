namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using Domain;

    public class ModelBaseWrapper<T> : ModelWrapper<T>
        where T : ModelBase
    {
        public ModelBaseWrapper(T model) : base(model)
        {
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Content
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}