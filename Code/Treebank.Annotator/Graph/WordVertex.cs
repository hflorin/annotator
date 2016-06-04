namespace Treebank.Annotator.Graph
{
    using System;
    using GraphX.PCL.Common.Models;
    using Wrapper;

    public class WordVertex : VertexBase
    {
        private readonly string vertexValueAttribute;
        private readonly WordWrapper wordWrapper;

        public WordVertex(WordWrapper wordWrapper, string vertexValueAttribute)
        {
            if (wordWrapper == null)
            {
                throw new ArgumentNullException("wordWrapper", @"Must provide an instance of WordWrapper");
            }

            this.wordWrapper = wordWrapper;
            this.vertexValueAttribute = string.IsNullOrEmpty(vertexValueAttribute) ? "form" : vertexValueAttribute;
            SetVertexId();
        }

        public WordWrapper WordWrapper
        {
            get { return wordWrapper; }
        }

        public string VertexLabel
        {
            get { return wordWrapper.GetAttributeByName(vertexValueAttribute); }
            set { wordWrapper.SetAttributeByName(vertexValueAttribute, value); }
        }

        private void SetVertexId()
        {
            ID = int.Parse(wordWrapper.GetAttributeByName("id"));
        }

        public override string ToString()
        {
            return VertexLabel;
        }
    }
}