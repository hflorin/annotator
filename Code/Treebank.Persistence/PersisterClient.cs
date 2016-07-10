namespace Treebank.Persistence
{
    using System;
    using Domain;

    public class PersisterClient
    {
        private readonly IPersister persister;

        public PersisterClient(IPersister persister)
        {
            if (persister == null)
            {
                throw new ArgumentNullException("persister");
            }

            this.persister = persister;
        }

        public void Save(Document document, string filepath)
        {
            persister.Save(document, filepath);
        }
    }
}