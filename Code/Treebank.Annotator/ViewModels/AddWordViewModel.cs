namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using Mappers;
    using Wrapper;
    using Attribute = Domain.Attribute;

    public class Pair
    {
        public int Id { get; set; }
        public string Form { get; set; }
    }

    public class AddWordViewModel : Observable
    {
        private readonly Word wordPrototype;
        private readonly Word wordPrototypeOriginal;

        private readonly List<Pair> words;

        public AddWordViewModel(Word wordPrototype, List<Pair> words)
        {
            if (wordPrototype == null)
            {
                throw new ArgumentNullException("wordPrototype");
            }
            if (words == null)
            {
                throw new ArgumentNullException("words");
            }

            this.wordPrototype = wordPrototype;
            wordPrototypeOriginal = ObjectCopier.Clone(wordPrototype);
            this.words = words;

            SetAllWordAttributesAsEditable(this.wordPrototype);
            SetAllowedValuesSetForWordIdAttribute(this.wordPrototype, this.words);
            SetAllowedValuesSetForHeadWordAttribute(this.wordPrototype, this.words);

            Word = new WordWrapper(this.wordPrototype);

            OkButtonCommand = new DelegateCommand(OkButtonCommandExecute, OkButtonCommandCanExecute);
        }

        public ICommand OkButtonCommand { get; set; }

        public WordWrapper Word { get; set; }

        private void SetAllowedValuesSetForHeadWordAttribute(Word word, List<Pair> wordsParam)
        {
            var localCopy = new List<Pair>(wordsParam) {new Pair {Id = 0, Form = string.Empty}};

            var headWordIdAttribute = word.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("head"));
            localCopy.Sort((l, r) => { return l.Id == r.Id ? 0 : l.Id < r.Id ? -1 : 1; });
            headWordIdAttribute.AllowedValuesSet = localCopy.Select(p => p.Form).ToList();
            headWordIdAttribute.Value = localCopy.Aggregate((l, r) => l.Id > r.Id ? l : r).Form;
        }

        private void OkButtonCommandExecute(object obj)
        {
            var wordFormValue = Word.Model.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("form")).Value;
            Word.Attributes.Add(
                new AttributeWrapper(new Attribute
                {
                    Name = "content",
                    DisplayName = "Content",
                    Value = wordFormValue,
                    IsEditable = false,
                    IsOptional = false
                }));
            var originalWordWrapper = new WordWrapper(wordPrototypeOriginal);

            var originalHeadWordAttribute =
                originalWordWrapper.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("head"));


            var headwordAttribute = Word.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("head"));
            

            headwordAttribute.IsEditable = originalHeadWordAttribute.IsEditable;
            headwordAttribute.AllowedValuesSet = originalHeadWordAttribute.AllowedValuesSet;

            var headWordId = words.Where(p => p.Form == headwordAttribute.Value).Select(p => p.Id).FirstOrDefault();
            headwordAttribute.Value = headWordId.ToString();
            Word.AcceptChanges();
            //todo:validate the values entered
        }

        private bool OkButtonCommandCanExecute(object arg)
        {
            return true;
        }

        private void SetAllowedValuesSetForWordIdAttribute(Word word, List<Pair> wordsParam)
        {
            var newId = wordsParam.Any() ? wordsParam.Max(w => w.Id) + 1 : 0;
            var idAttribute = word.Attributes.Single(a => a.Name.ToLowerInvariant().Equals("id"));
            idAttribute.IsEditable = false;
            idAttribute.Value = newId.ToString();
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