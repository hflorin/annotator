namespace Treebank.Events
{
    using System;
    using Prism.Events;

    public class ZoomOnWordIdentifier
    {
        public Guid ViewId { get; set; }
        public string WordId { get; set; }
    }

    public class ZoomOnWordVertexEvent : PubSubEvent<ZoomOnWordIdentifier>
    {
    }
}