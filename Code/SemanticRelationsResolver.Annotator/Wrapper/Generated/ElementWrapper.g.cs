namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class ElementWrapper<T> : ModelWrapper<T>
	where T : Domain.Element
	{
		public ElementWrapper(T model) : base(model)
		{
        }

		public System.String Name
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String NameOriginalValue
        {
            get { return GetOriginalValue<System.String>("Name"); }
        }

        public bool NameIsChanged
        {
            get { return GetIsChanged("Name"); }
        }

		public System.String DisplayName
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String DisplayNameOriginalValue
        {
            get { return GetOriginalValue<System.String>("DisplayName"); }
        }

        public bool DisplayNameIsChanged
        {
            get { return GetIsChanged("DisplayName"); }
        }

		public System.String Value
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String ValueOriginalValue
        {
            get { return GetOriginalValue<System.String>("Value"); }
        }

        public bool ValueIsChanged
        {
            get { return GetIsChanged("Value"); }
        }

		public System.Boolean IsOptional
        {
            get { return GetValue<System.Boolean>(); }
            set { SetValue(value); }
        }

        public System.Boolean IsOptionalOriginalValue
        {
            get { return GetOriginalValue<System.Boolean>("IsOptional"); }
        }

        public bool IsOptionalIsChanged
        {
            get { return GetIsChanged("IsOptional"); }
        }

		public ChangeTrackingCollection<AttributeWrapper> Attributes { get; set; }

		protected override void InitializeCollectionProperties(T model)
		{
			if(model == null)
			{
				throw new ArgumentException("T model instance cannot be null.");
			}

			base.InitializeCollectionProperties(model);

			if(model.Attributes == null)
			{
				throw new ArgumentException("Attributes cannot be null.");
			}
			Attributes = new ChangeTrackingCollection<AttributeWrapper>(model.Attributes.Select(e => new AttributeWrapper(e)));
			AddAttributesMetadata();
			RegisterCollection(Attributes, model.Attributes);
		}
	}
}
