namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class BasicTests
    {
        [SetUp]
        public void Initialize()
        {
            _sentence = new Sentence
            {
                Id = _sentenceId,
                Words = new List<Word>
                {
                    new Word
                    {
                        Id = _wordId
                    }
                }
            };
        }

        private readonly Guid _sentenceId = Guid.NewGuid();
        private readonly Guid _wordId = Guid.NewGuid();

        private Sentence _sentence;

        [Test]
        public void ShouldContainModelInModelProperty()
        {
            var wrapper = new SentenceWrapper(_sentence);
            Assert.AreEqual(_sentence, wrapper.Model);
        }

        [Test]
        public void ShouldSetValueOfUnderlyingModelProperty()
        {
            var wrapper = new SentenceWrapper(_sentence);

            var newWordId = Guid.NewGuid();

            wrapper.Words = new List<Word>
            {
                new Word
                {
                    Id = newWordId
                }
            };

            Assert.AreEqual(wrapper.Words.First().Id, newWordId);
        }

        [Test]
        public void ShouldThrowExceptionIfNullIsPassed()
        {
            Assert.Throws<ArgumentNullException>(() => new SentenceWrapper(null));
        }
    }
}