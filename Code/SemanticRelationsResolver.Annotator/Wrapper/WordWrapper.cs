﻿namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Base;
    using Domain;

    public class WordWrapper : ModelBaseWrapper<Word>
    {
        public WordWrapper(Word model)
            : base(model)
        {
        }

        [Required(ErrorMessage = @"Form is required.")]
        public string Form
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string FormOriginalValue
        {
            get { return GetOriginalValue<string>("Form"); }
        }

        public bool FormIsChanged
        {
            get { return GetIsChanged("Form"); }
        }

        public string Lemma
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LemmaOriginalValue
        {
            get { return GetOriginalValue<string>("Lemma"); }
        }

        public bool LemmaIsChanged
        {
            get { return GetIsChanged("Lemma"); }
        }

        public string PartOfSpeech
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PartOfSpeechOriginalValue
        {
            get { return GetOriginalValue<string>("PartOfSpeech"); }
        }

        public bool PartOfSpeechIsChanged
        {
            get { return GetIsChanged("PartOfSpeech"); }
        }

        public string Chunk
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ChunkOriginalValue
        {
            get { return GetOriginalValue<string>("Chunk"); }
        }

        public bool ChunkIsChanged
        {
            get { return GetIsChanged("Chunk"); }
        }

        public string DependencyRelation
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DependencyRelationOriginalValue
        {
            get { return GetOriginalValue<string>("DependencyRelation"); }
        }

        public bool DependencyRelationIsChanged
        {
            get { return GetIsChanged("DependencyRelation"); }
        }

        public int HeadWordId
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int HeadWordIdOriginalValue
        {
            get { return GetOriginalValue<int>("HeadWordId"); }
        }

        public bool HeadWordIdIsChanged
        {
            get { return GetIsChanged("HeadWordId"); }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PartOfSpeech == null || Form == null)
            {
                yield return
                    new ValidationResult("A word must have a part of speech and a form", new[] {"PartOfSpeech", "Form"});
            }
        }
    }
}