namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ViewModels;

    public class ModelWrapper<T> : Observable, IRevertibleChangeTracking
    {
        private readonly Dictionary<string, object> _originalValues;

        public ModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", @"Must provide a sentence model.");
            }
            Model = model;
            _originalValues = new Dictionary<string, object>();
        }

        public T Model { get; private set; }

        public bool IsChanged
        {
            get { return _originalValues.Count > 0; }
        }

        public void RejectChanges()
        {
            foreach (var originalValue in _originalValues)
            {
                typeof (T).GetProperty(originalValue.Key).SetValue(Model, originalValue.Value);
            }

            _originalValues.Clear();
            OnPropertyChanged(string.Empty);
        }

        public void AcceptChanges()
        {
            _originalValues.Clear();
            OnPropertyChanged(string.Empty);
        }

        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            var property = Model.GetType().GetProperty(propertyName);
            var currentValue = property.GetValue(Model);

            if (Equals(currentValue, newValue))
            {
                return;
            }

            UpdateOriginalValue(currentValue, newValue, propertyName);

            property.SetValue(Model, newValue);
            OnPropertyChanged(propertyName);
            OnPropertyChanged(propertyName + "IsChanged");
        }

        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged("IsChanged");
            }
            else
            {
                if (!Equals(_originalValues[propertyName], newValue))
                {
                    return;
                }
                _originalValues.Remove(propertyName);
                OnPropertyChanged("IsChanged");
            }
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default(TValue);
            }

            var property = Model.GetType().GetProperty(propertyName);
            return (TValue) property.GetValue(Model);
        }

        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default(TValue);
            }

            return _originalValues.ContainsKey(propertyName)
                ? (TValue) _originalValues[propertyName]
                : GetValue<TValue>(propertyName);
        }

        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        protected void RegisterCollection<TWrapper, TModel>(ObservableCollection<TWrapper> wrapperCollection,
            ICollection<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (sender, args) =>
            {
                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.OldItems.Cast<TWrapper>())
                    {
                        modelCollection.Remove(oldItem.Model);
                    }
                }

                if (args.NewItems == null)
                {
                    return;
                }

                foreach (var newItem in args.NewItems.Cast<TWrapper>())
                {
                    modelCollection.Add(newItem.Model);
                }
            };
        }
    }
}