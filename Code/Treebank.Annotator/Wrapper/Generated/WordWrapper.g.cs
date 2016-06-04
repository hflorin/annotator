namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Treebank.Domain;

    public partial class WordWrapper : Treebank.Annotator.Wrapper.ElementWrapper<Word>
	{
		public WordWrapper(Word model) : base(model)
		{
		}
	}
}
