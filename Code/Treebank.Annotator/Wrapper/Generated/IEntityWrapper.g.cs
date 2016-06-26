namespace Treebank.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using Treebank.Domain;

	public partial class IEntityWrapper : ElementWrapper<Domain.IEntity>
	{
		public IEntityWrapper(Domain.IEntity model) : base(model)
		{
		}
	}
}
