namespace SemanticRelationsResolver.UnitTests.Wrapper
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
            _sentence = DomainMother.Sentence;

            var word = DomainMother.Word;
            word.Attributes.Single(a => a.Name == "Id").Value = "1";
            _sentence.Words = new List<Word>
            {
                DomainMother.Word,
                word
            };
        }

        private Sentence _sentence;

        [Test]
        public void ShouldRaisePropertyChangedEventForIsValid()
        {
            var wrapper = new SentenceWrapper(_sentence);

            var fired = false;

            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsValid")
                {
                    fired = true;
                }
            };

            wrapper.Words.First().Form = string.Empty;
            Assert.IsTrue(fired);

            fired = false;
            wrapper.Words.First().Form = "form value";
            Assert.IsTrue(fired);
        }

        [Test]
        public void ShouldSetIsValidOfRoot()
        {
            var wrapper = new SentenceWrapper(_sentence);

            Assert.IsTrue(wrapper.IsValid);

            wrapper.Words.First().Form = string.Empty;

            Assert.IsFalse(wrapper.IsValid);

            wrapper.Words.First().Form = "new form";

            Assert.IsTrue(wrapper.IsValid);
        }
    }
}