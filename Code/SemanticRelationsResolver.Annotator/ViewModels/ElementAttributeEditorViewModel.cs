namespace SemanticRelationsResolver.Annotator.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Wrapper;
    using Wrapper.Base;
    using Attribute = Domain.Attribute;

    public class ElementAttributeEditorViewModel : Observable
    {
        public ElementAttributeEditorViewModel()
        {
            InitializeCommands();
        }

        public ChangeTrackingCollection<AttributeWrapper> Attributes { get; set; }

        public ICommand AddAttributeCommand { get; set; }

        public ICommand RemoveAttributeCommand { get; set; }

        public AttributeWrapper SelectedAttribute { get; set; }

        public bool AnyAttributes
        {
            get { return Attributes.Any(); }
        }

        public ICommand CancelAttributeCommand { get; set; }

        public ICommand SaveAttributeCommand { get; set; }

        private void InitializeCommands()
        {
            AddAttributeCommand = new DelegateCommand(AddAttributeCommandExecute, AddAttributeCommandCanExecute);
            RemoveAttributeCommand = new DelegateCommand(RemoveAttributeCommandExecute, RemoveAttributeCommandCanExecute);
            SaveAttributeCommand = new DelegateCommand(SaveAttributeCommandExecute, SaveAttributeCommandCanExecute);
            CancelAttributeCommand = new DelegateCommand(CancelAttributeCommandExecute, CancelAttributeCommandCanExecute);
        }

        private void CancelAttributeCommandExecute(object obj)
        {
            Attributes.RejectChanges();
        }

        private bool CancelAttributeCommandCanExecute(object arg)
        {
            return Attributes.IsChanged;
        }

        private void SaveAttributeCommandExecute(object obj)
        {
            //todo: save to the file as well
            Attributes.AcceptChanges();
        }

        private bool SaveAttributeCommandCanExecute(object arg)
        {
            return Attributes.IsChanged && Attributes.IsValid;
        }

        private void RemoveAttributeCommandExecute(object obj)
        {
            Attributes.Remove(SelectedAttribute);
        }

        private bool RemoveAttributeCommandCanExecute(object arg)
        {
            return (Attributes != null) && Attributes.Any();
        }

        private void AddAttributeCommandExecute(object obj)
        {
            //todo: display add attribute input window and then add the create attribute
            Attributes.Add(new AttributeWrapper(new Attribute
            {
                DisplayName = "Ciuciu",
                Name = "Muciu",
                Value = "Zewa"
            }));

            InvalidateCommands();
        }

        private bool AddAttributeCommandCanExecute(object arg)
        {
            return true;
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand) SaveAttributeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) CancelAttributeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) AddAttributeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) RemoveAttributeCommand).RaiseCanExecuteChanged();
        }
    }
}