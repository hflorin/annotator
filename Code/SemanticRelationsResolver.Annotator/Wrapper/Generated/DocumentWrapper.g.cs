namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class DocumentWrapper : ModelBaseWrapper<Document>
	{
		public DocumentWrapper(Document model) : base(model)
		{
		}

		public System.String Identifier
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String IdentifierOriginalValue
        {
            get { return GetOriginalValue<System.String>("Identifier"); }
        }

        public bool IdentifierIsChanged
        {
            get { return GetIsChanged("Identifier"); }
        }

		public ChangeTrackingCollection<SentenceWrapper> Sentences { get; set; }

		protected override void InitializeCollectionProperties(Document model)
		{
			if(model.Sentences == null)
			{
				throw new ArgumentException("Sentences cannot be null.");
			}
			Sentences = new ChangeTrackingCollection<SentenceWrapper>(model.Sentences.Select(e => new SentenceWrapper(e)));
			RegisterCollection(Sentences, model.Sentences);
		}
	}
}
