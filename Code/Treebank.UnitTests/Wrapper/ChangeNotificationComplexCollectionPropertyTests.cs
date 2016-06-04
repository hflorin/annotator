namespace Treebank.UnitTests.Wrapper
{
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class ChangeNotificationComplexCollectionPropertyTests
    {
        [SetUp]
        public void Initialize()
        {
            sentence = DomainMother.Sentence;
        }

        private Sentence sentence;
        private static readonly Word Word = DomainMother.Word;

        private void CheckModelWordsCollectionIsInSync(SentenceWrapper wrapper)
        {
            Assert.AreEqual(sentence.Words.Count, wrapper.Words.Count);
            Assert.IsTrue(
                sentence.Words.All(modelWord => wrapper.Words.Any(wrappedWord => wrappedWord.Model == modelWord)));
        }

        [Test]
        public void ShoudBeInSyncAfterClearingWords()
        {
            var wrapper = new SentenceWrapper(sentence);
            wrapper.Words.Clear();

            CheckModelWordsCollectionIsInSync(wrapper);
        }

        [Test]
        public void ShoudBeInSyncWhenAddingWords()
        {
            sentence.Words.Remove(Word);
            var wrapper = new SentenceWrapper(sentence);
            wrapper.Words.Add(new WordWrapper(Word));
            CheckModelWordsCollectionIsInSync(wrapper);
        }

        [Test]
        public void ShoudBeInSyncWhenRemovingWords()
        {
            var wrapper = new SentenceWrapper(sentence);
            var wordToRemove = wrapper.Words.Single(ww => ww.Model == Word);
            wrapper.Words.Remove(wordToRemove);

            CheckModelWordsCollectionIsInSync(wrapper);
        }

        [Test]
        public void ShoudInitializeWordsPropery()
        {
            var wrapper = new SentenceWrapper(sentence);
            Assert.IsNotNull(wrapper.Words);
            CheckModelWordsCollectionIsInSync(wrapper);
        }
    }
}