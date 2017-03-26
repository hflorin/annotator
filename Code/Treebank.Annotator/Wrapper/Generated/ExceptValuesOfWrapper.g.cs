namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using Treebank.Domain;

	public partial class ExceptValuesOfWrapper : ElementWrapper<ExceptValuesOf>
	{
		public ExceptValuesOfWrapper(ExceptValuesOf model) : base(model)
		{
		}

		public System.String ElementName
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String ElementNameOriginalValue
        {
            get { return GetOriginalValue<System.String>("ElementName"); }
        }

        public bool ElementNameIsChanged
        {
            get { return GetIsChanged("ElementName"); }
        }

		public ChangeTrackingCollection<StringWrapper> Values { get; set; }

		protected override void InitializeCollectionProperties(ExceptValuesOf model)
		{
			if(model == null)
			{
				throw new ArgumentException("Domain.ExceptValuesOf model instance cannot be null.");
			}

			base.InitializeCollectionProperties(model);

			if(model.Values == null)
			{
				throw new ArgumentException("Values cannot be null.");
			}
			Values = new ChangeTrackingCollection<StringWrapper>(model.Values.Select(e => new StringWrapper(e)));
			RegisterCollection(Values, model.Values);
		}
	}
}
