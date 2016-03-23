namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class WordWrapper : ElementWrapper<Domain.Word>
	{
		public WordWrapper(Domain.Word model) : base(model)
		{
		}
	}
}
