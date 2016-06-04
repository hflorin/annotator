namespace Treebank.UnitTests.Wrapper
{
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class ValidationCollectionProperty
    {
        [SetUp]
        public void Setup()
        {
            sentence = DomainMother.Sentence;

            var word = DomainMother.Word;
            word.Attributes.Single(a => a.Name == "Id").Value = "1";
            sentence.Words = new List<Word> {DomainMother.Word, word};
        }

        private Sentence sentence;

        [Test]
        public void ShouldRaisePropertyChangedEventForIsValid()
        {
            var wrapper = new SentenceWrapper(sentence);

            var fired = false;

            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsValid")
                {
                    fired = true;
                }
            };

            wrapper.Words.First().Attributes.Single(a => a.DisplayName.Equals("Form")).Value = string.Empty;
            Assert.IsTrue(fired);

            fired = false;
            wrapper.Words.First().Attributes.Single(a => a.DisplayName.Equals("Form")).Value = "form value";
            Assert.IsTrue(fired);
        }

        [Test]
        public void ShouldSetIsValidOfRoot()
        {
            var wrapper = new SentenceWrapper(sentence);

            Assert.IsTrue(wrapper.IsValid);

            wrapper.Words.First().Attributes.Single(a => a.DisplayName.Equals("Form")).Value = string.Empty;

            Assert.IsFalse(wrapper.IsValid);

            wrapper.Words.First().Attributes.Single(a => a.DisplayName.Equals("Form")).Value = "new form";

            Assert.IsTrue(wrapper.IsValid);
        }
    }
}