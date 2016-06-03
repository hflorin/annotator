namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Commands;
    using Domain;
    using View;
    using Wrapper;
    using Wrapper.Base;

    public class AddAttributeViewModel : Observable
    {
        public AddAttributeViewModel()
        {
            Attribute = new AttributeWrapper(new Attribute
            {
                IsEditable = true,
                IsOptional = true,
                AllowedValuesSet = new List<string>()
            });

            OkButtonCommand = new DelegateCommand(OkButtonCommandExecute, OkButtonCommandCanExecute);
            AddButtonCommand = new DelegateCommand(AddButtonCommandExecute, AddButtonCommandCanExecute);
            DeleteButtonCommand = new DelegateCommand(DeleteButtonCommandExecute, DeleteButtonCommandCanExecute);
        }

        public StringWrapper SelectedAllowedValue { get; set; }

        public ICommand DeleteButtonCommand { get; set; }

        public ICommand OkButtonCommand { get; set; }

        public ICommand AddButtonCommand { get; set; }

        public AttributeWrapper Attribute { get; set; }

        private void DeleteButtonCommandExecute(object obj)
        {
            Attribute.AllowedValuesSet.Remove(SelectedAllowedValue);
        }

        private bool DeleteButtonCommandCanExecute(object arg)
        {
            return true;
        }

        private void AddButtonCommandExecute(object obj)
        {
            var inputDialog = new InputDialog();
            if (inputDialog.ShowDialog().GetValueOrDefault())
            {
                Attribute.AllowedValuesSet.Add(new StringWrapper(inputDialog.Value));
            }
        }

        private bool AddButtonCommandCanExecute(object arg)
        {
            return true;
        }

        private void OkButtonCommandExecute(object obj)
        {
            Attribute.AcceptChanges();
        }

        private bool OkButtonCommandCanExecute(object arg)
        {
            return true;
        }
    }
}