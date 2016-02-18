namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System.Collections.Generic;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ChangeNotificationSimpleProperty
    {
        [SetUp]
        public void Initialize()
        {
            _sentence = new Sentence();
        }

        private Sentence _sentence;

        [Test]
        public void ShoudRaisePropertyChangedEventOnPropertyChange()
        {
            var wrapper = new SentenceWrapper(_sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Words")
                {
                    fired = true;
                }
            };

            wrapper.Words = new List<Word>();

            Assert.IsTrue(fired);
        }

        [Test]
        public void ShoudNotRaisePropertyChangedEventIfNewValueIsSameAsOldValue()
        {
            var wrapper = new SentenceWrapper(_sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Words")
                {
                    fired = true;
                }
            };

            wrapper.Words = _sentence.Words;

            Assert.IsFalse(fired);
        }
    }
}