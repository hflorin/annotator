namespace Treebank.UnitTests.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Domain;
    using Attribute = Domain.Attribute;

    public static class DomainMother
    {
        private const string ParserValue = "parser";

        private const string ContentValue = "content";

        private const string UserValue = "user";

        private const string IdValue = "0";

        private const string Chunk = "chunk";

        private const string DepRel = "DepRel";

        private const string Form = "form";

        private const string Lemma = "lemma";

        private const string Postag = "verb";

        private static readonly string DateValue = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        private static readonly ICollection<Word> WordsValue = new List<Word>();

        private static Sentence sentence;

        private static Word word;

        public static Sentence Sentence
        {
            get
            {
                sentence = new Sentence();
                sentence.Attributes.Add(new Attribute {Name = "parser", DisplayName = "Parser", Value = ParserValue});
                sentence.Attributes.Add(
                    new Attribute {Name = "content", DisplayName = "Content", Value = ContentValue});
                sentence.Attributes.Add(new Attribute {Name = "User", DisplayName = "User", Value = UserValue});
                sentence.Attributes.Add(new Attribute {Name = "Date", DisplayName = "Date", Value = DateValue});
                sentence.Attributes.Add(new Attribute {Name = "Id", DisplayName = "Id", Value = IdValue});
                sentence.Words = WordsValue;
                return sentence;
            }

            set { sentence = value; }
        }

        public static Word Word
        {
            get
            {
                word = new Word();
                word.Attributes.Add(new Attribute {Name = "Chunk", DisplayName = "Chunk", Value = Chunk});
                word.Attributes.Add(new Attribute {Name = "content", DisplayName = "Content", Value = ContentValue});
                word.Attributes.Add(new Attribute {Name = "Id", DisplayName = "Id", Value = IdValue});
                word.Attributes.Add(new Attribute {Name = "DepRel", DisplayName = "DepRel", Value = DepRel});
                word.Attributes.Add(new Attribute {Name = "Form", DisplayName = "Form", Value = Form});
                word.Attributes.Add(new Attribute {Name = "Head", DisplayName = "Head", Value = IdValue});
                word.Attributes.Add(new Attribute {Name = "Lemma", DisplayName = "Lemma", Value = Lemma});
                word.Attributes.Add(new Attribute {Name = "Postag", DisplayName = "Postag", Value = Postag});
                return word;
            }

            set { word = value; }
        }
    }
}