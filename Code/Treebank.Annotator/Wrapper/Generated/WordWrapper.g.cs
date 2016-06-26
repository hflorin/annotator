namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using Treebank.Domain;

	public partial class WordWrapper : ElementWrapper<Domain.Word>
	{
		public WordWrapper(Domain.Word model) : base(model)
		{
		}
	}
}
