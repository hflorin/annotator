namespace Treebank.Annotator.Wrapper
{
    using System;
    using System.Linq;
    using Treebank.Annotator.Wrapper.Base;
    using Treebank.Domain;

    public partial class SentenceWrapper : Treebank.Annotator.Wrapper.ElementWrapper<Sentence>
	{
		public SentenceWrapper(Sentence model) : base(model)
		{
		}

		public ChangeTrackingCollection<Treebank.Annotator.Wrapper.WordWrapper> Words { get; set; }

		protected override void InitializeCollectionProperties(Sentence model)
		{
			if(model == null)
			{
				throw new ArgumentException("Domain.Sentence model instance cannot be null.");
			}

			base.InitializeCollectionProperties(model);

			if(model.Words == null)
			{
				throw new ArgumentException("Words cannot be null.");
			}
			Words = new ChangeTrackingCollection<Treebank.Annotator.Wrapper.WordWrapper>(model.Words.Select(e => new Treebank.Annotator.Wrapper.WordWrapper(e)));
			RegisterCollection(Words, model.Words);
		}
	}
}
