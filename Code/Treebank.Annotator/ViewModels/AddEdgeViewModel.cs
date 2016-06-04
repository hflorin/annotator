namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Commands;
    using Wrapper;
    using Wrapper.Base;
    using Attribute = Domain.Attribute;

    public class AddEdgeViewModel : Observable
    {
        public AddEdgeViewModel(Attribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }
            attribute.IsEditable = true;
            Attributes =
                new ChangeTrackingCollection<AttributeWrapper>(new List<AttributeWrapper>
                {
                    new AttributeWrapper(attribute)
                });

            OkButtonCommand = new DelegateCommand(OkButtonCommandExecute, OkButtonCommandCanExecute);
        }

        public ICommand OkButtonCommand { get; set; }

        public ChangeTrackingCollection<AttributeWrapper> Attributes { get; set; }

        private void OkButtonCommandExecute(object obj)
        {
            Attributes.AcceptChanges();
        }

        private bool OkButtonCommandCanExecute(object arg)
        {
            return true;
        }
    }
}