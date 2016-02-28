namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using Base;
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

        public int IdOriginalValue
        {
            get { return GetOriginalValue<int>("Id"); }
        }

        public bool IdIsChanged
        {
            get { return GetIsChanged("Id"); }
        }

        public string Content
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ContentOriginalValue
        {
            get { return GetOriginalValue<string>("Content"); }
        }

        public bool ContentIsChanged
        {
            get { return GetIsChanged("Content"); }
        }
    }
}