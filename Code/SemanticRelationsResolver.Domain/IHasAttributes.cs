namespace SemanticRelationsResolver.Domain
{
    using System.Collections.Generic;

    public interface IHasAttributes
    {
        ICollection<Attribute> Attributes { get; set; }
    }
}