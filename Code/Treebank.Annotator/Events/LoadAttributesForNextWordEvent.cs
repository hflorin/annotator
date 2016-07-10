namespace Treebank.Annotator.Events
{
    using System;
    using Prism.Events;

    public enum Directions
    {
        Next,
        Previous
    }

    public class NextElementAttributesRequest
    {
        public Guid ViewId { get; set; }
        public string ElementId { get; set; }
        public Directions Direction { get; set; }
    }

    public class LoadAttributesForNextWordEvent : PubSubEvent<NextElementAttributesRequest>
    {
    }
}