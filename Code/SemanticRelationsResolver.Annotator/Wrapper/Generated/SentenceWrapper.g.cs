namespace SemanticRelationsResolver.Annotator.Wrapper
{
	using System;
	using System.Linq;
	using Base;
	using SemanticRelationsResolver.Domain;

	public partial class SentenceWrapper : ModelBaseWrapper<Sentence>
	{
		public SentenceWrapper(Sentence model) : base(model)
		{
		}

		public System.String Parser
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String ParserOriginalValue
        {
            get { return GetOriginalValue<System.String>("Parser"); }
        }

        public bool ParserIsChanged
        {
            get { return GetIsChanged("Parser"); }
        }

		public System.String User
        {
            get { return GetValue<System.String>(); }
            set { SetValue(value); }
        }

        public System.String UserOriginalValue
        {
            get { return GetOriginalValue<System.String>("User"); }
        }

        public bool UserIsChanged
        {
            get { return GetIsChanged("User"); }
        }

		public System.DateTime Date
        {
            get { return GetValue<System.DateTime>(); }
            set { SetValue(value); }
        }

        public System.DateTime DateOriginalValue
        {
            get { return GetOriginalValue<System.DateTime>("Date"); }
        }

        public bool DateIsChanged
        {
            get { return GetIsChanged("Date"); }
        }

		public ChangeTrackingCollection<WordWrapper> Words { get; set; }

		protected override void InitializeCollectionProperties(Sentence model)
		{
			if(model.Words == null)
			{
				throw new ArgumentException("Words cannot be null.");
			}
			Words = new ChangeTrackingCollection<WordWrapper>(model.Words.Select(e => new WordWrapper(e)));
			RegisterCollection(Words, model.Words);
		}
	}
}