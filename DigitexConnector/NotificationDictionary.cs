using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DigitexConnector
{
    public class NotificationDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        private ConcurrentDictionary<TKey, TValue> backingDictionary;

        // This is the only method that absolutely must be overridden,
        // because without it the KeyedCollection cannot extract the
        // keys from the items. 
        //
        public NotificationDictionary() 
        {
            backingDictionary = new ConcurrentDictionary<TKey, TValue>();
            OnPropertyChanged("");
        }

        public void Add(TKey key, TValue order)
        {
            AddNotification(key, order);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pair)
        {
            AddNotification(pair.Key, pair.Value);
        }

        public bool ContainsKey(TKey key) 
        {
            return backingDictionary.ContainsKey(key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> pair)
        {
            return backingDictionary.ContainsKey(pair.Key);
        }

        public bool TryGetValue(TKey key, out TValue order) {
            return backingDictionary.TryGetValue(key, out order);
        }

        public bool Remove(TKey key)
        {
            return RemoveWithNotification(key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> pair)
        {
            return RemoveWithNotification(pair.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)backingDictionary).GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)backingDictionary).GetEnumerator();
        }

        public void Clear()
        {
            backingDictionary.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            OnPropertyChanged("Count");
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() 
        {
            backingDictionary.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            OnPropertyChanged("Count");
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)backingDictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return backingDictionary.Count; }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return backingDictionary.Count; }
        }

        public ICollection<TKey> Keys 
        {
            get { return backingDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return backingDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            set { UpdateWithNotification(key, value); }
            get { return backingDictionary[key]; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)backingDictionary).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)backingDictionary).IsReadOnly; }
        }

        void AddNotification(TKey key, TValue order)
        {
            bool result = backingDictionary.TryAdd(key, order);
            if (result)
            {
                OnPropertyChanged("Count");
                OnPropertyChanged("Keys");
                OnPropertyChanged("Values");
                OnCollectionChanged(NotifyCollectionChangedAction.Add,
                    new KeyValuePair<TKey, TValue>(key, order));
            }
        }

        bool RemoveWithNotification(TKey key)
        {
            try{
                TValue order = backingDictionary[key];
                //bool flag = backingDictionary.TryGetValue(key, out order);
                bool result = backingDictionary.TryRemove(key, out order);
                //if ( flag && backingDictionary.TryRemove(key, out order))
                if (result)
                {
                    OnPropertyChanged("Count");
                    OnPropertyChanged("Keys");
                    OnPropertyChanged("Values");
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove,
                        new KeyValuePair<TKey, TValue>(key, order));
                    return true;
                }
                return false;
            } catch
            {
                return false;
            }
        }

        void UpdateWithNotification(TKey key, TValue order)
        {
            TValue existing = backingDictionary[key];
            if (backingDictionary.TryGetValue(key, out existing))
            {
                backingDictionary[key] = order;
                OnPropertyChanged("Values");
                OnCollectionChanged(NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<TKey, TValue>(key, order),
                    new KeyValuePair<TKey, TValue>(key, existing));
            }
            else
            {
                AddNotification(key, order);
            }
        }

        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void OnCollectionChanged(NotifyCollectionChangedAction act, KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(), KeyValuePair<TKey, TValue> existing = new KeyValuePair<TKey, TValue>())
        {
            if (CollectionChanged == null)
            {
                return;
            }
            if (act == NotifyCollectionChangedAction.Remove || act == NotifyCollectionChangedAction.Add)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(act, pair));
            }
            else if (act == NotifyCollectionChangedAction.Replace)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(act, pair, existing));
            }
            else if (act == NotifyCollectionChangedAction.Reset)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(act));
            }
        }
    }
}
