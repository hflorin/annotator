namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using Treebank.Domain;

	public partial class DocumentWrapper : ElementWrapper<Domain.Document>
	{
		public DocumentWrapper(Domain.Document model) : base(model)
		{
		}

		public System.String FilePath
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String FilePathOriginalValue
        {
            get { return GetOriginalValue<System.String>("FilePath"); }
        }

        public bool FilePathIsChanged
        {
            get { return GetIsChanged("FilePath"); }
        }

		public ChangeTrackingCollection<SentenceWrapper> Sentences { get; set; }

		protected override void InitializeCollectionProperties(Domain.Document model)
		{
			if(model == null)
			{
				throw new ArgumentException("Domain.Document model instance cannot be null.");
			}

			base.InitializeCollectionProperties(model);

			if(model.Sentences == null)
			{
				throw new ArgumentException("Sentences cannot be null.");
			}
			Sentences = new ChangeTrackingCollection<SentenceWrapper>(model.Sentences.Select(e => new SentenceWrapper(e)));
			RegisterCollection(Sentences, model.Sentences);
		}
	}
}
