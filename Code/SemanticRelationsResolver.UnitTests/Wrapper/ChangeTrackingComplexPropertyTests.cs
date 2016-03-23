namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class ChangeTrackingComplexPropertyTests
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

            wrapper.Attributes.Single(a=> a.Name == "Parser").Value = "new parser";

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

            wrapper.Attributes.Single(a=>a.Name=="Parser").Value = "new parser";

            Assert.IsTrue(fired);
        }

        [Test]
        public void ShouldAcceptChanges()
        {
            var wrapper = new SentenceWrapper(sentence);

            wrapper.Attributes.Single(a => a.Name == "Parser").Value = "new value";

            Assert.AreEqual("new value", wrapper.Attributes.Single(a=>a.Name=="Parser").Value);
            Assert.AreEqual(parserValue, wrapper.Attributes.Single(a => a.Name == "Parser").ValueOriginalValue);
            Assert.IsTrue(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.AreEqual("new value", wrapper.Attributes.Single(a => a.Name == "Parser").Value);
            Assert.AreEqual("new value", wrapper.Attributes.Single(a => a.Name == "Parser").ValueOriginalValue);
            Assert.IsFalse(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldRejectChanges()
        {
            var wrapper = new SentenceWrapper(sentence);

            wrapper.Attributes.Single(a => a.Name == "Parser").Value = "new value";

            Assert.AreEqual("new value", wrapper.Attributes.Single(a => a.Name == "Parser").Value);
            Assert.AreEqual(parserValue, wrapper.Attributes.Single(a => a.Name == "Parser").ValueOriginalValue);
            Assert.IsTrue(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual(parserValue, wrapper.Attributes.Single(a => a.Name == "Parser").Value);
            Assert.AreEqual(parserValue, wrapper.Attributes.Single(a => a.Name == "Parser").ValueOriginalValue);
            Assert.IsFalse(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldSetIsChanged()
        {
            var wrapper = new SentenceWrapper(sentence);
            Assert.IsFalse(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsFalse(wrapper.IsChanged);

            wrapper.Attributes.Single(a => a.Name == "Parser").Value = "new value";
            Assert.IsTrue(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.Attributes.Single(a => a.Name == "Parser").Value = parserValue;
            Assert.IsFalse(wrapper.Attributes.Single(a => a.Name == "Parser").ValueIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldStoreOriginalValue()
        {
            var wrapper = new SentenceWrapper(sentence);
            
            Assert.AreEqual(parserValue, wrapper.Attributes.Single(a => a.Name == "Parser").ValueOriginalValue);

            wrapper.Attributes.Single(a => a.Name == "Parser").Value = "new value";

            Assert.AreEqual(parserValue, wrapper.Attributes.Single(a => a.Name == "Parser").ValueOriginalValue);
        }
    }
}