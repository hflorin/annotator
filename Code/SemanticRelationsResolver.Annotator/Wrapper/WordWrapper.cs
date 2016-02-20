namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using Domain;

    public class WordWrapper : ModelBaseWrapper<Word>
    {
        public WordWrapper(Word model)
            : base(model)
        {
        }

        public string Form
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Lemma
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PartOfSpeech
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Chunk
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DependencyRelation
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int HeadWordId
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}