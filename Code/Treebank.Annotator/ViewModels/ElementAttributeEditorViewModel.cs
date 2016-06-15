namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Prism.Events;
    using Treebank.Events;
    using View;
    using Wrapper;
    using Wrapper.Base;

    public class ElementAttributeEditorViewModel : Observable
    {
        private readonly IEventAggregator eventAggregator;
        public Guid ViewId { get; set; }

        public ElementAttributeEditorViewModel(IEventAggregator eventAggregator, Guid viewId)
        {
            InitializeCommands();
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            ViewId = viewId;
            this.eventAggregator = eventAggregator;
        }

        public IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
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
            eventAggregator.GetEvent<GenerateGraphEvent>().Publish(ViewId);
        }

        private bool SaveAttributeCommandCanExecute(object arg)
        {
            return Attributes.IsChanged && Attributes.IsValid;
        }

        private void RemoveAttributeCommandExecute(object obj)
        {
            if (SelectedAttribute!=null && SelectedAttribute.IsOptional)
            {
                Attributes.Remove(SelectedAttribute);
                InvalidateCommands();
            }
            else
            {
                eventAggregator.GetEvent<StatusNotificationEvent>()
                    .Publish(string.Format("Cannot delete attribute {0} because it is not optional.",
                        SelectedAttribute.Name));
            }
        }

        private bool RemoveAttributeCommandCanExecute(object arg)
        {
            return (Attributes != null) && Attributes.Any();
        }

        private void AddAttributeCommandExecute(object obj)
        {
            var addAttributeWindow = new AddAttributeWindow(new AddAttributeViewModel());

            if (!addAttributeWindow.ShowDialog().GetValueOrDefault())
                return;

            var dataContext = addAttributeWindow.DataContext as AddAttributeViewModel;
            if (dataContext != null)
            {
                Attributes.Add(dataContext.Attribute);
            }

            InvalidateCommands();
        }

        private bool AddAttributeCommandCanExecute(object arg)
        {
            return true;
        }

        public void InvalidateCommands()
        {
            ((DelegateCommand) SaveAttributeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) CancelAttributeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) AddAttributeCommand).RaiseCanExecuteChanged();
            ((DelegateCommand) RemoveAttributeCommand).RaiseCanExecuteChanged();
        }
    }
}