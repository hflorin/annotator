namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ViewModels;

    public class ModelWrapper<T> : Observable
    {
        public ModelWrapper(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", @"Must provide a sentence model.");
            }
            Model = model;
        }

        public T Model { get; private set; }

        protected void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            var property = Model.GetType().GetProperty(propertyName);
            var currentValue = property.GetValue(Model);

            if (Equals(currentValue, value))
            {
                return;
            }

            property.SetValue(Model, value);
            OnPropertyChanged(propertyName);
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

                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems.Cast<TWrapper>())
                    {
                        modelCollection.Add(newItem.Model);
                    }
                }
            };
        }
    }
}