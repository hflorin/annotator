namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public class ModelBaseWrapper<T> : ModelWrapper<T>
	where T : ModelBase
	{
		public ModelBaseWrapper(T model) : base(model)
		{
        }

		public System.String Id
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String IdOriginalValue
        {
            get { return GetOriginalValue<System.String>("Id"); }
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
