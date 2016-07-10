namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Events;
    using Mappers.Configuration;
    using Prism.Events;
    using Treebank.Events;
    using View;
    using Wrapper;
    using Wrapper.Base;

    public class ElementAttributeEditorViewModel : Observable
    {
        private readonly IEventAggregator eventAggregator;

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

        public Guid ViewId { get; set; }

        public IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
        }

        public ChangeTrackingCollection<AttributeWrapper> Attributes { get; set; }

        public ICommand AddAttributeCommand { get; set; }

        public ICommand RemoveAttributeCommand { get; set; }

        public ICommand NextAttributesCommand { get; set; }

        public ICommand PreviousAttributesCommand { get; set; }

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
            NextAttributesCommand = new DelegateCommand(NextAttributesCommandExecute, NextAttributesCommandCanExecute);
            PreviousAttributesCommand = new DelegateCommand(PreviousAttributesCommandExecute, PreviousAttributesCommandCanExecute);
        }

        private void PreviousAttributesCommandExecute(object obj)
        {
            var idAttribute = Attributes.FirstOrDefault(a => a.Name == "id");

            if (idAttribute != null)
            {
                eventAggregator.GetEvent<LoadAttributesForNextWordEvent>().Publish(new NextElementAttributesRequest
                {
                    ViewId = ViewId,
                    ElementId = idAttribute.Value,
                    Direction = Directions.Previous
                });
            }
        }

        private bool PreviousAttributesCommandCanExecute(object arg)
        {
            return true;
        }

        private bool NextAttributesCommandCanExecute(object arg)
        {
            return true;
        }

        private void NextAttributesCommandExecute(object obj)
        {
            var idAttribute = Attributes.FirstOrDefault(a => a.Name == "id");

            if (idAttribute != null)
            {
                eventAggregator.GetEvent<LoadAttributesForNextWordEvent>().Publish(new NextElementAttributesRequest
                {
                    ViewId = ViewId,
                    ElementId = idAttribute.Value,
                    Direction = Directions.Next
                });
            }
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
            var configurationAttribute = Attributes.FirstOrDefault(a => a.Name == "configuration");
            if (configurationAttribute != null)
            {
                var configurationFilePathAttribute = Attributes.FirstOrDefault(a => a.Name == "configurationFilePath");
                if (configurationFilePathAttribute != null)
                {
                    var name = Path.GetFileName(configurationFilePathAttribute.Value);

                    if (name != configurationAttribute.Value)
                    {
                        var mapping = AppConfig.GetConfigFileNameToFilePathMapping();

                        var pair = mapping.FirstOrDefault(p => p.Value == configurationAttribute.Value);

                        if (!string.IsNullOrWhiteSpace(pair.Key))
                        {
                            configurationFilePathAttribute.Value = pair.Key;
                        }
                    }
                }
            }


            Attributes.AcceptChanges();
            eventAggregator.GetEvent<UpdateAllViewsForSentenceByViewId>().Publish(ViewId);
        }

        private bool SaveAttributeCommandCanExecute(object arg)
        {
            return Attributes.IsChanged && Attributes.IsValid;
        }

        private void RemoveAttributeCommandExecute(object obj)
        {
            if ((SelectedAttribute != null) && SelectedAttribute.IsOptional)
            {
                Attributes.Remove(SelectedAttribute);
                InvalidateCommands();
            }
            else
            {
                if (SelectedAttribute != null)
                {
                    eventAggregator.GetEvent<StatusNotificationEvent>()
                        .Publish(string.Format("Cannot delete attribute {0} because it is not optional.",
                            SelectedAttribute.Name));
                }
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