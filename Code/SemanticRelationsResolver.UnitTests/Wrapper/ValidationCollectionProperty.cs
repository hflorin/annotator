namespace SemanticRelationsResolver.UnitTests.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Annotator.Wrapper;
    using Domain;
    using NUnit.Framework;

    [TestFixture]
    public class ValidationCollectionProperty
    {
        [SetUp]
        public void Setup()
        {
            _sentence = new Sentence
            {
                Content = "content",
                Date = DateTime.Now,
                Id = "0",
                Parser = "parser",
                User = "user",
                Words = new List<Word>
                {
                    new Word
                    {
                        Chunk = "chunk",
                        Content = "content",
                        Id = "0",
                        DependencyRelation = "relaion",
                        Form = "form",
                        HeadWordId = "0",
                        Lemma = "lemma",
                        PartOfSpeech = "verb"
                    },
                    new Word
                    {
                        Chunk = "chunk",
                        Content = "content",
                        Id = "1",
                        DependencyRelation = "relaion",
                        Form = "form",
                        HeadWordId = "0",
                        Lemma = "lemma",
                        PartOfSpeech = "verb"
                    }
                }
            };
        }

        private Sentence _sentence;

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
    }
}