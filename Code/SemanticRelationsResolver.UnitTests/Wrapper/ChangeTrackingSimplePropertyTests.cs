namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class ChangeTrackingSimplePropertyTests
    {
        [SetUp]
        public void Initialize()
        {
            sentence = DomainMother.Sentence;

            parserValue = sentence.Attributes.Single(a => a.DisplayName == "Parser").Value;
        }

        private string parserValue;
        private Sentence sentence;

        [Test]
        public void ShoudRaisePropertyChangedEventForIsChanged()
        {
            var wrapper = new SentenceWrapper(sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsChanged")
                {
                    fired = true;
                }
            };

            wrapper.Parser = "new parser";

            Assert.IsTrue(fired);
        }

        [Test]
        public void ShoudRaisePropertyChangedEventForParserIsChanged()
        {
            var wrapper = new SentenceWrapper(sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ParserIsChanged")
                {
                    fired = true;
                }
            };

            wrapper.Parser = "new parser";

            Assert.IsTrue(fired);
        }

        [Test]
        public void ShouldAcceptChanges()
        {
            var wrapper = new SentenceWrapper(sentence) {Parser = "new value"};
            Assert.AreEqual("new value", wrapper.Parser);
            Assert.AreEqual(parserValue, wrapper.ParserOriginalValue);
            Assert.IsTrue(wrapper.ParserIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.AreEqual("new value", wrapper.Parser);
            Assert.AreEqual("new value", wrapper.ParserOriginalValue);
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldRejectChanges()
        {
            var wrapper = new SentenceWrapper(sentence) {Parser = "new value"};
            Assert.AreEqual("new value", wrapper.Parser);
            Assert.AreEqual(parserValue, wrapper.ParserOriginalValue);
            Assert.IsTrue(wrapper.ParserIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual(parserValue, wrapper.Parser);
            Assert.AreEqual(parserValue, wrapper.ParserOriginalValue);
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldSetIsChanged()
        {
            var wrapper = new SentenceWrapper(sentence);
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);

            wrapper.Parser = "new value";
            Assert.IsTrue(wrapper.ParserIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.Parser = parserValue;
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldStoreOriginalValue()
        {
            var wrapper = new SentenceWrapper(sentence);

            Assert.AreEqual(parserValue, wrapper.ParserOriginalValue);

            wrapper.Parser = "new value";

            Assert.AreEqual(parserValue, wrapper.ParserOriginalValue);
        }
    }
}