namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class DynamicDocumentWrapper : ModelBaseWrapper<DynamicDocument>
	{
		public DynamicDocumentWrapper(DynamicDocument model) : base(model)
		{
		}
	}
}
