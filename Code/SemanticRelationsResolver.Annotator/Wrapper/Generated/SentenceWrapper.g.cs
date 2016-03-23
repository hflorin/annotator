namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class SentenceWrapper : ElementWrapper<Domain.Sentence>
	{
		public SentenceWrapper(Domain.Sentence model) : base(model)
		{
		}

		public ChangeTrackingCollection<WordWrapper> Words { get; set; }

		protected override void InitializeCollectionProperties(Domain.Sentence model)
		{
			if(model.Words == null)
			{
				throw new ArgumentException("Words cannot be null.");
			}
			Words = new ChangeTrackingCollection<WordWrapper>(model.Words.Select(e => new WordWrapper(e)));
			RegisterCollection(Words, model.Words);
		}
	}
}
