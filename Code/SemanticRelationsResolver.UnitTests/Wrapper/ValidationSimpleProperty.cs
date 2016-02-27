namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ValidationSimpleProperty
    {
        [SetUp]
        public void Setup()
        {
            _sentence = new Sentence
            {
                Content = "content",
                Date = DateTime.Now,
                Id = 0,
                Parser = "parser",
                User = "user",
                Words = new List<Word>()
            };
        }

        private Sentence _sentence;

        [Test]
        public void ShouldRaiseErrorsChangedEventWhenParserIsSetToEmpty()
        {
            var wrapper = new SentenceWrapper(_sentence);

            var fired = false;

            wrapper.ErrorsChanged += (s, e) =>
            {
                if (e.PropertyName == "Parser")
                {
                    fired = true;
                }
            };

            wrapper.Parser = string.Empty;

            Assert.IsTrue(fired);

            fired = false;
            wrapper.Parser = "parser value";

            Assert.IsFalse(wrapper.HasErrors);
        }

        [Test]
        public void ShouldReturnValidationErrorWhenParserIsSetToEmptyAnd()
        {
            var wrapper = new SentenceWrapper(_sentence);

            Assert.IsFalse(wrapper.HasErrors);

            wrapper.Parser = string.Empty;

            Assert.IsTrue(wrapper.HasErrors);

            var errors = (List<string>)wrapper.GetErrors("Parser");

            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Parser is required.", errors.First());

            wrapper.Parser = "parser value";

            Assert.IsFalse(wrapper.HasErrors);
        }
    }
}