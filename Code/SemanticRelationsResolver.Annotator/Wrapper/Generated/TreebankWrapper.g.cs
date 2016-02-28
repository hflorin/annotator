namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class TreebankWrapper : ModelBaseWrapper<Treebank>
	{
		public TreebankWrapper(Treebank model) : base(model)
		{
		}
	}
}
