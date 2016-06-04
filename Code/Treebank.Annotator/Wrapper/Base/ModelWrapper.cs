namespace Treebank.Annotator.Wrapper.Base
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IValidatableTrackingObject, IValidatableObject
    {
        private readonly Dictionary<string, object> _originalValues;

        private readonly List<IValidatableTrackingObject> _trackingObjects;

        public ModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", @"Must provide a sentence model.");
            }
            Model = model;
            _trackingObjects = new List<IValidatableTrackingObject>();
            _originalValues = new Dictionary<string, object>();
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
            Validate();
        }

        protected virtual void InitializeComplexProperties(T model)
        {
        }

        protected virtual void InitializeCollectionProperties(T model)
        {
        }

        public T Model { get; private set; }

        public bool IsValid
        {
            get { return !HasErrors && _trackingObjects.All(t => t.IsValid); }
        }

        public bool IsChanged
        {
            get { return _originalValues.Count > 0 || _trackingObjects.Any(o => o.IsChanged); }
        }

        public void RejectChanges()
        {
            foreach (var originalValue in _originalValues)
            {
                typeof (T).GetProperty(originalValue.Key).SetValue(Model, originalValue.Value);
            }
            _originalValues.Clear();

            foreach (var trackingObject in _trackingObjects)
            {
                trackingObject.RejectChanges();
            }
            Validate();
            OnPropertyChanged(string.Empty);
        }

        public void AcceptChanges()
        {
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
            {
                trackingObject.AcceptChanges();
            }
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

            Validate();

            OnPropertyChanged(propertyName);
            OnPropertyChanged(propertyName + "IsChanged");
        }

        private void Validate()
        {
            ClearErrors();

            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);

            Validator.TryValidateObject(this, context, results, true);

            if (results.Any())
            {
                var propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();
                foreach (var propertyName in propertyNames)
                {
                    Errors[propertyName] =
                        results.Where(r => r.MemberNames.Contains(propertyName))
                            .Select(r => r.ErrorMessage)
                            .Distinct()
                            .ToList();
                    OnErrorsChanged(propertyName);
                }
            }
            OnPropertyChanged("IsValid");
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

        protected void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection,
            ICollection<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (sender, args) =>
            {
                modelCollection.Clear();
                foreach (var model in wrapperCollection)
                {
                    modelCollection.Add(model.Model);
                }
                Validate();
            };

            RegisterTrackingObject(wrapperCollection);
        }

        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObject(wrapper);
        }

        private void RegisterTrackingObject(IValidatableTrackingObject trackingObject)
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
            {
                OnPropertyChanged("IsChanged");
            }
            else if (e.PropertyName == "IsValid")
            {
                OnPropertyChanged("IsValid");
            }
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}