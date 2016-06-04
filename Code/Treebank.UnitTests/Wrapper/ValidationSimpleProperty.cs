namespace Treebank.UnitTests.Wrapper
{
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class ValidationSimpleProperty
    {
        [SetUp]
        public void Setup()
        {
            sentence = DomainMother.Sentence;
        }

        private Sentence sentence;

        [Test]
        public void ShouldRaiseErrorsChangedEventWhenParserIsSetToEmpty()
        {
            var wrapper = new SentenceWrapper(sentence);

            var fired = false;

            wrapper.ErrorsChanged += (s, e) =>
            {
                if (e.PropertyName == "Parser")
                {
                    fired = true;
                }
            };

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = string.Empty;

            Assert.IsTrue(fired);

            fired = false;
            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = "parser value";

            Assert.IsFalse(wrapper.HasErrors);
        }

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

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = string.Empty;
            Assert.IsTrue(fired);

            fired = false;
            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = "parser values";
            Assert.IsTrue(fired);
        }

        [Test]
        public void ShouldRefreshErrorsAndIsValidWhenRejectingChanges()
        {
            var wrapper = new SentenceWrapper(sentence);

            Assert.IsTrue(wrapper.IsValid);
            Assert.IsFalse(wrapper.HasErrors);

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = string.Empty;
            Assert.IsFalse(wrapper.IsValid);
            Assert.IsTrue(wrapper.HasErrors);

            wrapper.RejectChanges();

            Assert.IsTrue(wrapper.IsValid);
            Assert.IsFalse(wrapper.HasErrors);
        }

        [Test]
        public void ShouldReturnValidationErrorWhenParserIsSetToEmptyAnd()
        {
            var wrapper = new SentenceWrapper(sentence);

            Assert.IsFalse(wrapper.HasErrors);

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = string.Empty;

            Assert.IsTrue(wrapper.HasErrors);

            var errors = (List<string>) wrapper.GetErrors("Parser");

            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Parser is required.", errors.First());

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = "parser value";

            Assert.IsFalse(wrapper.HasErrors);
        }

        [Test]
        public void ShouldSetErrorsAndIsValidAfterInitialization()
        {
            sentence.Attributes.Single(a => a.DisplayName == "Parser").Value = string.Empty;
            var wrapper = new SentenceWrapper(sentence);

            Assert.IsFalse(wrapper.IsValid);
            Assert.IsTrue(wrapper.HasErrors);

            var errors = (List<string>) wrapper.GetErrors("Parser");
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Parser is required.", errors.First());
        }

        [Test]
        public void ShouldSetIsValid()
        {
            var wrapper = new SentenceWrapper(sentence);

            Assert.IsTrue(wrapper.IsValid);

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = string.Empty;
            Assert.IsFalse(wrapper.IsValid);

            wrapper.Attributes.Single(a => a.DisplayName == "Parser").Value = "parser value";
            Assert.IsTrue(wrapper.IsValid);
        }
    }
}