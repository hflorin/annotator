namespace SemanticRelationsResolver.UnitTests.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Domain;
    using Attribute = Domain.Attribute;

    public static class DomainMother
    {
        private static readonly string _parserValue = "parser";
        private static readonly string _contentValue = "content";
        private static readonly string _userValue = "user";
        private static readonly string DateValue = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        private static readonly string _idValue = "0";
        private static readonly string _content = "content";
        private static readonly ICollection<Word> WordsValue = new List<Word>();

        private static readonly string _chunk = "chunk";
        private static readonly string _depRel = "DepRel";
        private static readonly string _form = "form";
        private static readonly string _lemma = "lemma";
        private static readonly string _postag = "verb";

        private static Sentence sentence;

        private static Word word;

        public static Sentence Sentence
        {
            get
            {
                sentence = new Sentence();
                sentence.Attributes.Add(new Attribute {Name = "parser", DisplayName = "Parser", Value = _parserValue});
                sentence.Attributes.Add(new Attribute {Name = "Content", DisplayName = "Content", Value = _contentValue});
                sentence.Attributes.Add(new Attribute {Name = "User", DisplayName = "User", Value = _userValue});
                sentence.Attributes.Add(new Attribute {Name = "Date", DisplayName = "Date", Value = DateValue});
                sentence.Attributes.Add(new Attribute {Name = "Id", DisplayName = "Id", Value = _idValue});
                sentence.Attributes.Add(new Attribute {Name = "Content", DisplayName = "Content", Value = _content});
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
                word.Attributes.Add(new Attribute {Name = "Chunk", DisplayName = "Chunk", Value = _chunk});
                word.Attributes.Add(new Attribute {Name = "Content", DisplayName = "Content", Value = _contentValue});
                word.Attributes.Add(new Attribute {Name = "Id", DisplayName = "Id", Value = _idValue});
                word.Attributes.Add(new Attribute {Name = "DepRel", DisplayName = "DepRel", Value = _depRel});
                word.Attributes.Add(new Attribute {Name = "Form", DisplayName = "Form", Value = _form});
                word.Attributes.Add(new Attribute {Name = "Head", DisplayName = "Head", Value = _idValue});
                word.Attributes.Add(new Attribute {Name = "Lemma", DisplayName = "Lemma", Value = _lemma});
                word.Attributes.Add(new Attribute {Name = "Postag", DisplayName = "Postag", Value = _postag});
                return word;
            }
            set { word = value; }
        }
    }
}