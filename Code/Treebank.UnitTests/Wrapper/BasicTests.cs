namespace Treebank.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Annotator.Wrapper.Base;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class BasicTests
    {
        [SetUp]
        public void Initialize()
        {
            sentence = DomainMother.Sentence;
        }

        private Sentence sentence;

        [Test]
        public void ShouldContainModelInModelProperty()
        {
            var wrapper = new SentenceWrapper(sentence);
            Assert.AreEqual(sentence, wrapper.Model);
        }

        [Test]
        public void ShouldSetValueOfUnderlyingModelProperty()
        {
            var wrapper = new SentenceWrapper(sentence);

            const string newWordId = "2";

            var newWord = DomainMother.Word;
            newWord.Attributes.Single(a => a.Name == "Id").Value = newWordId;

            wrapper.Words = new ChangeTrackingCollection<WordWrapper>(new List<WordWrapper>
            {
                new WordWrapper(newWord)
            });

            Assert.AreEqual(wrapper.Words.First().Attributes.Single(a => a.DisplayName == "Id").Value, newWordId);
        }

        [Test]
        public void ShouldThrowExceptionIfNullIsPassed()
        {
            Assert.Throws<ArgumentNullException>(() => new SentenceWrapper(null));
        }
    }
}