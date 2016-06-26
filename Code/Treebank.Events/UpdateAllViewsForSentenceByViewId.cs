namespace Treebank.Events
{
    using System;
    using Prism.Events;

    public class UpdateAllViewsForSentenceByViewId : PubSubEvent<Guid>
    {
    }
}