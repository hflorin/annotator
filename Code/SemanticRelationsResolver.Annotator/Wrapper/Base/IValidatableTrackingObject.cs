namespace SemanticRelationsResolver.Annotator.Wrapper.Base
{
    using System.ComponentModel;

    public interface IValidatableTrackingObject : INotifyPropertyChanged, IRevertibleChangeTracking
    {
        bool IsValid { get; }
    }
}