namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Annotator.Wrapper.Base;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ChangeTrackingCollectionPropertyTests
    {
        [SetUp]
        public void Initialize()
        {
            _words = new List<WordWrapper>
            {
                new WordWrapper(new Word {Id = 0}),
                new WordWrapper(new Word {Id = 1})
            };
        }

        private ICollection<WordWrapper> _words;

        [Test]
        public void ShoudTrackAddedItems()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.IsChanged);

            collection.Add(wordToAdd);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(1, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.AreEqual(wordToAdd, collection.AddedItems.First());
            Assert.IsTrue(collection.IsChanged);

            collection.Remove(wordToAdd);

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(0, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.IsFalse(collection.IsChanged);
        }

        [Test]
        public void ShoudTrackRemoveItems()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.IsChanged);

            collection.Add(wordToAdd);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(1, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.AreEqual(wordToAdd, collection.AddedItems.First());
            Assert.IsTrue(collection.IsChanged);

            collection.Remove(wordToAdd);

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(0, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.IsFalse(collection.IsChanged);
        }

        [Test]
        public void ShoudTrackModifiedItems()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.IsChanged);

            collection.Add(wordToAdd);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(1, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.AreEqual(wordToAdd, collection.AddedItems.First());
            Assert.IsTrue(collection.IsChanged);

            collection.Remove(wordToAdd);

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(0, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.IsFalse(collection.IsChanged);
        }

        [Test]
        public void ShoudNotTrackAddedItemAsModified()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.IsChanged);

            collection.Add(wordToAdd);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(1, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.AreEqual(wordToAdd, collection.AddedItems.First());
            Assert.IsTrue(collection.IsChanged);

            collection.Remove(wordToAdd);

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(0, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.IsFalse(collection.IsChanged);
        }

        [Test]
        public void ShoudNotTrackRemovedItemAsModified()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);

            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.IsChanged);

            collection.Add(wordToAdd);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(1, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.AreEqual(wordToAdd, collection.AddedItems.First());
            Assert.IsTrue(collection.IsChanged);

            collection.Remove(wordToAdd);

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(0, collection.AddedItems.Count);
            Assert.AreEqual(0, collection.RemovedItems.Count);
            Assert.AreEqual(0, collection.ModifiedItems.Count);
            Assert.IsFalse(collection.IsChanged);
        }

        [Test]
        public void ShouldAcceptChanges()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);
        }

        [Test]
        public void ShouldRejectChanges()
        {
            var wordToAdd = new WordWrapper(new Word());
            var collection = new ChangeTrackingCollection<WordWrapper>(_words);
        }
    }
}