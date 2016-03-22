namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class ChangeNotificationSimplePropertyTests
    {
        [SetUp]
        public void Initialize()
        {
            sentence = DomainMother.Sentence;
        }

        private Sentence sentence;

        [Test]
        public void ShoudNotRaisePropertyChangedEventIfNewValueIsSameAsOldValue()
        {
            var wrapper = new SentenceWrapper(sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Parser")
                {
                    fired = true;
                }
            };

            wrapper.Parser = sentence.Attributes.Single(a => a.DisplayName == "Parser").Value;

            Assert.IsFalse(fired);
        }

        [Test]
        public void ShoudRaisePropertyChangedEventOnPropertyChange()
        {
            var wrapper = new SentenceWrapper(sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Parser")
                {
                    fired = true;
                }
            };

            wrapper.Parser = "New Value";

            Assert.IsTrue(fired);
        }
    }
}