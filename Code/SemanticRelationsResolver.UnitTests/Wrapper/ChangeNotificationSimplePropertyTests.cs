namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ChangeNotificationSimplePropertyTests
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
        private readonly int _idValue = 0;
        private readonly ICollection<Word> _wordsValue = new List<Word>();

        private Sentence _sentence;

        [Test]
        public void ShoudNotRaisePropertyChangedEventIfNewValueIsSameAsOldValue()
        {
            var wrapper = new SentenceWrapper(_sentence);

            var fired = false;

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Parser")
                {
                    fired = true;
                }
            };

            wrapper.Parser = _parserValue;

            Assert.IsFalse(fired);
        }

        [Test]
        public void ShoudRaisePropertyChangedEventOnPropertyChange()
        {
            var wrapper = new SentenceWrapper(_sentence);

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