namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ChangeTrackingSimplePropertyTests
    {
        [SetUp]
        public void Initialize()
        {
            _sentence = new Sentence
            {
                Parser = _parserValue,
                Content = _contentValue,
                User = _userValue,
                Date = _dateValue,
                Id = _idValue,
                Words = _wordsValue
            };
        }

        private readonly string _parserValue = "parser";
        private readonly string _contentValue = "content";
        private readonly string _userValue = "user";
        private readonly DateTime _dateValue = DateTime.Now;
        private readonly string _idValue = "0";
        private readonly ICollection<Word> _wordsValue = new List<Word>();

        private Sentence _sentence;

        [Test]
        public void ShoudRaisePropertyChangedEventForIsChanged()
        {
            var wrapper = new SentenceWrapper(_sentence);

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
            var wrapper = new SentenceWrapper(_sentence);

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
            var wrapper = new SentenceWrapper(_sentence) {Parser = "new value"};
            Assert.AreEqual("new value", wrapper.Parser);
            Assert.AreEqual(_parserValue, wrapper.ParserOriginalValue);
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
            var wrapper = new SentenceWrapper(_sentence) {Parser = "new value"};
            Assert.AreEqual("new value", wrapper.Parser);
            Assert.AreEqual(_parserValue, wrapper.ParserOriginalValue);
            Assert.IsTrue(wrapper.ParserIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual(_parserValue, wrapper.Parser);
            Assert.AreEqual(_parserValue, wrapper.ParserOriginalValue);
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldSetIsChanged()
        {
            var wrapper = new SentenceWrapper(_sentence);
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);

            wrapper.Parser = "new value";
            Assert.IsTrue(wrapper.ParserIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.Parser = _parserValue;
            Assert.IsFalse(wrapper.ParserIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [Test]
        public void ShouldStoreOriginalValue()
        {
            var wrapper = new SentenceWrapper(_sentence);

            Assert.AreEqual(_parserValue, wrapper.ParserOriginalValue);

            wrapper.Parser = "new value";

            Assert.AreEqual(_parserValue, wrapper.ParserOriginalValue);
        }
    }
}