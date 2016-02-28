namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public class DocumentWrapper : ModelBaseWrapper<Document>
	{
		public DocumentWrapper(Document model) : base(model)
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
