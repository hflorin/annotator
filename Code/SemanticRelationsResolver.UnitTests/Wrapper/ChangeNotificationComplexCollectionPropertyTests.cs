namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ChangeNotificationComplexCollectionPropertyTests
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

        private readonly ICollection<Word> _wordsValue = new List<Word>
        {
            Word,
            new Word
            {
                Id = "1"
            }
        };

        private Sentence _sentence;

        private static readonly Word Word = new Word
        {
            Id = "0"
        };

        private void CheckModelWordsCollectionIsInSync(SentenceWrapper wrapper)
        {
            Assert.AreEqual(_sentence.Words.Count, wrapper.Words.Count);
            Assert.IsTrue(
                _sentence.Words.All(modelWord => wrapper.Words.Any(wrappedWord => wrappedWord.Model == modelWord)));
        }

        [Test]
        public void ShoudBeInSyncWhenAddingWords()
        {
            _sentence.Words.Remove(Word);
            var wrapper = new SentenceWrapper(_sentence);
            wrapper.Words.Add(new WordWrapper(Word));
            CheckModelWordsCollectionIsInSync(wrapper);
        }

        [Test]
        public void ShoudBeInSyncWhenRemovingWords()
        {
            var wrapper = new SentenceWrapper(_sentence);
            var wordToRemove = wrapper.Words.Single(ww => ww.Model == Word);
            wrapper.Words.Remove(wordToRemove);

            CheckModelWordsCollectionIsInSync(wrapper);
        }

        [Test]
        public void ShoudBeInSyncAfterClearingWords()
        {
            var wrapper = new SentenceWrapper(_sentence);
            wrapper.Words.Clear();

            CheckModelWordsCollectionIsInSync(wrapper);
        }

        [Test]
        public void ShoudInitializeWordsPropery()
        {
            var wrapper = new SentenceWrapper(_sentence);
            Assert.IsNotNull(wrapper.Words);
            CheckModelWordsCollectionIsInSync(wrapper);
        }
    }
}