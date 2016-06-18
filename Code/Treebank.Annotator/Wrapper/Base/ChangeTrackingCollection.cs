namespace Treebank.Annotator.Wrapper.Base
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IValidatableTrackingObject
        where T : class, IValidatableTrackingObject
    {
        private readonly ObservableCollection<T> _addedItems;
        private readonly ObservableCollection<T> _modifiedItems;
        private readonly ObservableCollection<T> _removedItems;
        private IList<T> _originalItems;

        public ChangeTrackingCollection(IEnumerable<T> items) : base(items)
        {
            _originalItems = this.ToList();

            AttachItemPropertyChangedHandler(_originalItems);

            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);
        }

        public ReadOnlyObservableCollection<T> AddedItems { get; private set; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; private set; }
        public ReadOnlyObservableCollection<T> ModifiedItems { get; private set; }

        public void AcceptChanges()
        {
            _addedItems.Clear();
            _removedItems.Clear();
            _modifiedItems.Clear();

            foreach (var item in this)
            {
                item.AcceptChanges();
            }

            _originalItems = this.ToList();
            OnPropertyChanged(new PropertyChangedEventArgs("IsChanged"));
        }

        public bool IsChanged
        {
            get { return (AddedItems.Count > 0) || (RemovedItems.Count > 0) || (ModifiedItems.Count > 0); }
        }

        public void RejectChanges()
        {
            foreach (var addedItem in _addedItems.ToList())
            {
                Remove(addedItem);
            }

            foreach (var removedItem in _removedItems.ToList())
            {
                Add(removedItem);
            }

            foreach (var modifiedItem in _modifiedItems.ToList())
            {
                modifiedItem.RejectChanges();
            }

            OnPropertyChanged(new PropertyChangedEventArgs("IsChanged"));
        }

        public bool IsValid
        {
            get { return this.All(t => t.IsValid); }
        }

        private void AttachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid")
            {
                OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
            }
            else
            {
                var item = (T) sender;
                if (_addedItems.Contains(item))
                {
                    return;
                }

                if (item.IsChanged)
                {
                    if (!_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Add(item);
                    }
                }
                else
                {
                    if (_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Remove(item);
                    }
                }
                OnPropertyChanged(new PropertyChangedEventArgs("IsChanged"));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var added = this.Where(current => _originalItems.All(orig => orig != current));
            var removed = _originalItems.Where(orig => this.All(current => current != orig));

            var modified = this.Except(added).Except(removed).Where(item => item.IsChanged).ToList();

            AttachItemPropertyChangedHandler(added);
            DetachItemPropertyChangedHandler(removed.ToList());

            UpdateObservableCollection(_addedItems, added);
            UpdateObservableCollection(_removedItems, removed);
            UpdateObservableCollection(_modifiedItems, modified);

            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs("IsChanged"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
        }

        private void UpdateObservableCollection(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        private void DetachItemPropertyChangedHandler(List<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }
    }
}