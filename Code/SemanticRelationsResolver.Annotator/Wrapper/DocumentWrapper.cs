namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Linq;
    using Domain;

    public class DocumentWrapper : ModelBaseWrapper<Document>
    {
        public DocumentWrapper(Document model) : base(model)
        {
            InitializeCollectionProperty(model);
        }

        public ChangeTrackingCollection<SentenceWrapper> Sentences { get; set; }

        private void InitializeCollectionProperty(Document model)
        {
            if (model.Sentences == null)
            {
                throw new ArgumentException("Sentences cannot be null.");
            }

            Sentences =
                new ChangeTrackingCollection<SentenceWrapper>(
                    model.Sentences.Select(sentence => new SentenceWrapper(sentence)));

            RegisterCollection(Sentences, model.Sentences);
        }
    }
}