namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Treebank.Annotator.Wrapper.Base;
	using Treebank.Domain;

    public partial class DocumentWrapper : Treebank.Annotator.Wrapper.ElementWrapper<Document>
	{
		public DocumentWrapper(Document model) : base(model)
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

		public ChangeTrackingCollection<Treebank.Annotator.Wrapper.SentenceWrapper> Sentences { get; set; }

		protected override void InitializeCollectionProperties(Document model)
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
			Sentences = new ChangeTrackingCollection<Treebank.Annotator.Wrapper.SentenceWrapper>(model.Sentences.Select(e => new Treebank.Annotator.Wrapper.SentenceWrapper(e)));
			RegisterCollection(Sentences, model.Sentences);
		}
	}
}
