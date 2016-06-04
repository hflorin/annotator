namespace Treebank.Annotator.Wrapper.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Treebank.Annotator.ViewModels;

    public class NotifyDataErrorInfoBase : Observable, INotifyDataErrorInfo
    {
        protected readonly Dictionary<string, List<string>> Errors;

        protected NotifyDataErrorInfoBase()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return propertyName != null && Errors.ContainsKey(propertyName)
                ? Errors[propertyName]
                : Enumerable.Empty<string>();
        }

        public bool HasErrors
        {
            get { return Errors.Any(); }
        }

        protected void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected void ClearErrors()
        {
            foreach (var propertyName in Errors.Keys.ToList())
            {
                Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}