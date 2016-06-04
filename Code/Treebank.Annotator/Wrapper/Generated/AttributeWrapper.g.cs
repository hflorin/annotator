namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Treebank.Annotator.Wrapper.Base;
	using Attribute = Treebank.Domain.Attribute;

    public partial class AttributeWrapper : ModelWrapper<Attribute>
	{
		public AttributeWrapper(Attribute model) : base(model)
		{
        }

		public System.Boolean IsEditable
        {
            get { return GetValue<System.Boolean>(); }
            set { SetValue(value); }
        }

        public System.Boolean IsEditableOriginalValue
        {
            get { return GetOriginalValue<System.Boolean>("IsEditable"); }
        }

        public bool IsEditableIsChanged
        {
            get { return GetIsChanged("IsEditable"); }
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

        public System.String Entity
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String EntityOriginalValue
        {
            get { return GetOriginalValue<System.String>("Entity"); }
        }

        public bool EntityIsChanged
        {
            get { return GetIsChanged("Entity"); }
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

		public ChangeTrackingCollection<StringWrapper> AllowedValuesSet { get; set; }

		protected override void InitializeCollectionProperties(Attribute model)
		{
			if(model == null)
			{
				throw new ArgumentException("Domain.Attribute model instance cannot be null.");
			}

			base.InitializeCollectionProperties(model);

			if(model.AllowedValuesSet == null)
			{
				throw new ArgumentException("AllowedValuesSet cannot be null.");
			}
			AllowedValuesSet = new ChangeTrackingCollection<StringWrapper>(model.AllowedValuesSet.Select(e => new StringWrapper(e)));
			RegisterCollection(AllowedValuesSet, model.AllowedValuesSet);
		}
	}
}