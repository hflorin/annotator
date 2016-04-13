namespace SemanticRelationsResolver.Annotator.Graph
{
    using System;
    using GraphX.PCL.Common.Models;
    using Wrapper;

    public class WordVertex : VertexBase
    {
        private readonly WordWrapper wordWrapper;

        public WordVertex(WordWrapper wordWrapper)
        {
            if (wordWrapper == null)
            {
                throw new ArgumentNullException("wordWrapper", @"Must provide an instance of WordWrapper");
            }

            this.wordWrapper = wordWrapper;
            SetVertexId();
        }

        public string Form
        {
            get { return wordWrapper.GetAttributeByName("form"); }
            set { wordWrapper.SetAttributeByName("form", value); }
        }

        private void SetVertexId()
        {
            ID = int.Parse(wordWrapper.GetAttributeByName("id"));
        }

        public override string ToString()
        {
            return Form;
        }
    }
}