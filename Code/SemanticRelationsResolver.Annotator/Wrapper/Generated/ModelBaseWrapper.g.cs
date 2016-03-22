namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public class ModelBaseWrapper<T> : ModelWrapper<T>
	where T : Element
	{
		public ModelBaseWrapper(T model) : base(model)
		{
        }

		public System.Int32 Id
        {
            get { return GetValue<System.Int32>(); }
            set { SetValue(value); }
        }

        public System.Int32 IdOriginalValue
        {
            get { return GetOriginalValue<System.Int32>("Id"); }
        }

        public bool IdIsChanged
        {
            get { return GetIsChanged("Id"); }
        }

		public System.String Content
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String ContentOriginalValue
        {
            get { return GetOriginalValue<System.String>("Content"); }
        }

        public bool ContentIsChanged
        {
            get { return GetIsChanged("Content"); }
        }
	}
}
