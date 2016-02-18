namespace SemanticRelationsResolver.Annotator.Wrapper
{
    using System;
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
    }
}