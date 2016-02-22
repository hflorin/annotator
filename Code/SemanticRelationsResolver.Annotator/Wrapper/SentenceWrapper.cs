namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Linq;
    using Domain;

    public class SentenceWrapper : ModelBaseWrapper<Sentence>
    {
        public SentenceWrapper(Sentence model) : base(model)
        {
            InitializeCollectionProperty(model);
        }

        public string Parser
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ParserOriginalValue
        {
            get { return GetOriginalValue<string>("Parser"); }
        }

        public bool ParserIsChanged
        {
            get { return GetIsChanged("Parser"); }
        }

        public string User
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string UserOriginalValue
        {
            get { return GetOriginalValue<string>("User"); }
        }

        public bool UserIsChanged
        {
            get { return GetIsChanged("User"); }
        }

        public DateTime Date
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime DateOriginalValue
        {
            get { return GetOriginalValue<DateTime>("Date"); }
        }

        public bool DateIsChanged
        {
            get { return GetIsChanged("Date"); }
        }

        public ChangeTrackingCollection<WordWrapper> Words { get; set; }

        private void InitializeCollectionProperty(Sentence model)
        {
            if (model.Words == null)
            {
                throw new ArgumentException("Words cannot be null.");
            }

            Words = new ChangeTrackingCollection<WordWrapper>(model.Words.Select(word => new WordWrapper(word)));

            RegisterCollection(Words, model.Words);
        }
    }
}