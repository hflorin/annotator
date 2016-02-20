namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Collections.ObjectModel;
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

        public string User
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public DateTime Date
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<WordWrapper> Words { get; set; }

        private void InitializeCollectionProperty(Sentence model)
        {
            if (model.Words == null)
            {
                throw new ArgumentException("Words cannot be null.");
            }

            Words = new ObservableCollection<WordWrapper>(model.Words.Select(word => new WordWrapper(word)));

            RegisterCollection(Words, model.Words);
        }
    }
}