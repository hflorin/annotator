namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Collections.Generic;
    using Domain;

    public class SentenceWrapper : ModelWrapper<Sentence>
    {
        public SentenceWrapper(Sentence model) : base(model)
        {
        }

        public Guid Id
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public ICollection<Word> Words
        {
            get { return GetValue<ICollection<Word>>(); }
            set { SetValue(value); }
        }
    }
}