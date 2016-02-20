namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
                Id = SentenceId,
                Words = new List<Word>
                {
                    new Word
                    {
                        Id = WordId
                    }
                }
            };
        }

        private const int SentenceId = 0;
        private const int WordId = 1;

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

            const int newWordId = 2;

            wrapper.Words = new ObservableCollection<WordWrapper>
            {
                new WordWrapper(new Word
                {
                    Id = newWordId
                })
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