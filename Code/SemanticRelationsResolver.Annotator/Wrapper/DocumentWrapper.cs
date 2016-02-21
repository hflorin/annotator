namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Domain;

    public class DocumentWrapper : ModelBaseWrapper<Document>
    {
        public DocumentWrapper(Document model) : base(model)
        {
            InitializeCollectionProperty(model);
        }

        public ObservableCollection<SentenceWrapper> Sentences { get; set; }

        public ObservableCollection<SentenceWrapper> SentencesOriginalValue
        {
            get { return GetOriginalValue<ObservableCollection<SentenceWrapper>>("Sentences"); }
        }

        public bool SentencesIsChanged
        {
            get { return GetIsChanged("Sentences"); }
        }

        private void InitializeCollectionProperty(Document model)
        {
            if (model.Sentences == null)
            {
                throw new ArgumentException("Sentences cannot be null.");
            }

            Sentences =
                new ObservableCollection<SentenceWrapper>(
                    model.Sentences.Select(sentence => new SentenceWrapper(sentence)));

            RegisterCollection(Sentences, model.Sentences);
        }
    }
}