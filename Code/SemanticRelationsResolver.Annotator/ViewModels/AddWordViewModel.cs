namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using Wrapper;

    public class AddWordViewModel : Observable
    {
        private readonly Word wordPrototype;

        private readonly List<int> wordsIds;

        public AddWordViewModel(Word wordPrototype, List<int> existingWordIds)
        {
            if (wordPrototype == null)
            {
                throw new ArgumentNullException("wordPrototype");
            }
            if (existingWordIds == null)
            {
                throw new ArgumentNullException("existingWordIds");
            }

            this.wordPrototype = wordPrototype;
            wordsIds = existingWordIds;

            SetAllWordAttributesAsEditable(this.wordPrototype);
            SetAllowedValuesSetForWordIdAttribute(this.wordPrototype, wordsIds);
            SetAllowedValuesSetForHeadWordIdAttribute(this.wordPrototype, wordsIds);

            Word = new WordWrapper(this.wordPrototype);

            OkButtonCommand = new DelegateCommand(OkButtonCommandExecute, OkButtonCommandCanExecute);
        }

        public ICommand OkButtonCommand { get; set; }

        public WordWrapper Word { get; set; }

        private void SetAllowedValuesSetForHeadWordIdAttribute(Word word, List<int> allowedIds)
        {
            var localCopy = new List<int>(allowedIds) {0};


            var headWordIdAttribute = word.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("head"));
            localCopy.Sort();
            headWordIdAttribute.AllowedValuesSet = localCopy.Select(id => id.ToString()).ToList();
        }

        private void OkButtonCommandExecute(object obj)
        {
        }

        private bool OkButtonCommandCanExecute(object arg)
        {
            return true;
        }

        private void SetAllowedValuesSetForWordIdAttribute(Word word, List<int> allowedIds)
        {
            //supplement the list of available ids by one, for the new word
            var localCopy = new List<int>(allowedIds) {wordsIds.Max() + 1};

            var idAttribute = word.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("id"));
            localCopy.Sort();
            idAttribute.AllowedValuesSet = localCopy.Select(id => id.ToString()).ToList();
        }

        private void SetAllWordAttributesAsEditable(Word word)
        {
            foreach (var attribute in word.Attributes)
            {
                attribute.IsEditable = true;
            }
        }
    }
}