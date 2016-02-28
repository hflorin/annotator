namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class WordWrapper : ModelBaseWrapper<Word>
	{
		public WordWrapper(Word model) : base(model)
		{
		}

		public System.String Form
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String FormOriginalValue
        {
            get { return GetOriginalValue<System.String>("Form"); }
        }

        public bool FormIsChanged
        {
            get { return GetIsChanged("Form"); }
        }

		public System.String Lemma
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String LemmaOriginalValue
        {
            get { return GetOriginalValue<System.String>("Lemma"); }
        }

        public bool LemmaIsChanged
        {
            get { return GetIsChanged("Lemma"); }
        }

		public System.String PartOfSpeech
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String PartOfSpeechOriginalValue
        {
            get { return GetOriginalValue<System.String>("PartOfSpeech"); }
        }

        public bool PartOfSpeechIsChanged
        {
            get { return GetIsChanged("PartOfSpeech"); }
        }

		public System.Int32 HeadWordId
        {
            get { return GetValue<System.Int32>(); }
            set { SetValue(value); }
        }

        public System.Int32 HeadWordIdOriginalValue
        {
            get { return GetOriginalValue<System.Int32>("HeadWordId"); }
        }

        public bool HeadWordIdIsChanged
        {
            get { return GetIsChanged("HeadWordId"); }
        }

		public System.String Chunk
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String ChunkOriginalValue
        {
            get { return GetOriginalValue<System.String>("Chunk"); }
        }

        public bool ChunkIsChanged
        {
            get { return GetIsChanged("Chunk"); }
        }

		public System.String DependencyRelation
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String DependencyRelationOriginalValue
        {
            get { return GetOriginalValue<System.String>("DependencyRelation"); }
        }

        public bool DependencyRelationIsChanged
        {
            get { return GetIsChanged("DependencyRelation"); }
        }
	}
}
