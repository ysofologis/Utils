using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// code found at http://blogs.microsoft.co.il/blogs/shimmy/archive/2010/12/26/observabledictionary-lt-tkey-tvalue-gt-c.aspx
   /// </summary>
   /// <typeparam name="TKey"></typeparam>
   /// <typeparam name="TValue"></typeparam>
   public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
   {
      /// <summary>
      /// 
      /// </summary>
      protected class DictionaryLock : IDisposable
      {
         /// <summary>
         /// 
         /// </summary>
         protected object m_lockRef;
         /// <summary>
         /// 
         /// </summary>
         /// <param name="lref"></param>
         public DictionaryLock(object lref)
         {
            m_lockRef = lref;

            this.enterLock(m_lockRef);
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="lockRef"></param>
         protected virtual void enterLock(object lockRef) { ; }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="lockRef"></param>
         protected virtual void exitLock(object lockRef) { ; }
         /// <summary>
         /// 
         /// </summary>
         public void Dispose()
         {
            exitLock(m_lockRef);

            m_lockRef = null;
         }
      }

      private const string CountString = "Count";
      private const string IndexerName = "Item[]";
      private const string KeysName = "Keys";
      private const string ValuesName = "Values";
      /// <summary>
      /// 
      /// </summary>
      private IDictionary<TKey, TValue> m_dict;
      /// <summary>
      /// 
      /// </summary>
      protected IDictionary<TKey, TValue> Dictionary
      {
         get { return m_dict; }
      }

      #region Constructors
      /// <summary>
      /// 
      /// </summary>
      public ObservableDictionary()
      {
         this.enableNotifications = true;

         m_dict = new Dictionary<TKey, TValue>();
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="dictionary"></param>
      public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
      {
         this.enableNotifications = true;

         m_dict = new Dictionary<TKey, TValue>(dictionary);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="comparer"></param>
      public ObservableDictionary(IEqualityComparer<TKey> comparer)
      {
         this.enableNotifications = true;

         m_dict = new Dictionary<TKey, TValue>(comparer);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="capacity"></param>
      public ObservableDictionary(int capacity)
      {
         this.enableNotifications = true;

         m_dict = new Dictionary<TKey, TValue>(capacity);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="dictionary"></param>
      /// <param name="comparer"></param>
      public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
      {
         this.enableNotifications = true;

         m_dict = new Dictionary<TKey, TValue>(dictionary, comparer);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="capacity"></param>
      /// <param name="comparer"></param>
      public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
      {
         this.enableNotifications = true;

         m_dict = new Dictionary<TKey, TValue>(capacity, comparer);
      }
      #endregion
      /// <summary>
      /// 
      /// </summary>
      public bool enableNotifications { get; set; }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      protected virtual DictionaryLock createLock() { return new DictionaryLock(this); }

      #region IDictionary<TKey,TValue> Members
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <param name="value"></param>
      public void Add(TKey key, TValue value)
      {
         using (var l = this.createLock())
         {
            Insert(key, value, true);
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public bool ContainsKey(TKey key)
      {
         using (var l = this.createLock())
         {
            return Dictionary.ContainsKey(key);
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public ICollection<TKey> Keys
      {
         get
         {
            using (var l = this.createLock())
            {
               return Dictionary.Keys;
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public bool Remove(TKey key)
      {
         using (var l = this.createLock())
         {
            if (key == null) throw new ArgumentNullException("key");

            TValue value;
            Dictionary.TryGetValue(key, out value);
            var removed = Dictionary.Remove(key);
            if (removed)
               //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
               OnCollectionChanged();
            return removed;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      public bool TryGetValue(TKey key, out TValue value)
      {
         using (var l = this.createLock())
         {
            return Dictionary.TryGetValue(key, out value);
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public ICollection<TValue> Values
      {
         get
         {
            using (var l = this.createLock())
            {
               return Dictionary.Values;
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      public TValue this[TKey key]
      {
         get
         {

            using (var l = this.createLock())
            {
               return getItem(key);
            }
         }
         set
         {
            using (var l = this.createLock())
            {
               Insert(key, value, false);
            }
         }
      }

      #endregion

      protected virtual void itemRequested(TKey key, TValue item)
      {
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      protected TValue getItem(TKey key)
      {
         if (this.ContainsKey(key))
         {
            var item = this.Dictionary[key];

            itemRequested(key, item);

            return item;
         }
         else
         {
            TValue newItem = createItem(key);

            Insert(key, newItem, true);

            return newItem;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <returns></returns>
      protected virtual TValue createItem(TKey key)
      {
         throw new IndexOutOfRangeException(string.Format("Key [{0}] not found in the dictionary.", key));
      }

      #region ICollection<KeyValuePair<TKey,TValue>> Members
      /// <summary>
      /// 
      /// </summary>
      /// <param name="item"></param>
      public void Add(KeyValuePair<TKey, TValue> item)
      {
         Insert(item.Key, item.Value, true);
      }
      /// <summary>
      /// 
      /// </summary>
      public void Clear()
      {
         if (Dictionary.Count > 0)
         {
            Dictionary.Clear();
            OnCollectionChanged();
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public bool Contains(KeyValuePair<TKey, TValue> item)
      {
         return Dictionary.Contains(item);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="array"></param>
      /// <param name="arrayIndex"></param>
      public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
      {
         Dictionary.CopyTo(array, arrayIndex);
      }
      /// <summary>
      /// 
      /// </summary>
      public int Count
      {
         get { return Dictionary.Count; }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool IsReadOnly
      {
         get { return Dictionary.IsReadOnly; }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      public bool Remove(KeyValuePair<TKey, TValue> item)
      {
         return Remove(item.Key);
      }


      #endregion

      #region IEnumerable<KeyValuePair<TKey,TValue>> Members
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
      {
         return Dictionary.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
         return ((IEnumerable)Dictionary).GetEnumerator();
      }

      #endregion

      #region INotifyCollectionChanged Members
      /// <summary>
      /// 
      /// </summary>
      public event NotifyCollectionChangedEventHandler CollectionChanged;

      #endregion

      #region INotifyPropertyChanged Members
      /// <summary>
      /// 
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      #endregion
      /// <summary>
      /// 
      /// </summary>
      /// <param name="items"></param>
      public void AddRange(IDictionary<TKey, TValue> items)
      {
         if (items == null) throw new ArgumentNullException("items");

         if (items.Count > 0)
         {
            if (Dictionary.Count > 0)
            {
               if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                  throw new ArgumentException("An item with the same key has already been added.");
               else
                  foreach (var item in items) Dictionary.Add(item);
            }
            else
               m_dict = new Dictionary<TKey, TValue>(items);

            OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="key"></param>
      /// <param name="value"></param>
      /// <param name="add"></param>
      protected void Insert(TKey key, TValue value, bool add)
      {
         if (key == null) throw new ArgumentNullException("key");

         TValue item;
         if (Dictionary.TryGetValue(key, out item))
         {
            if (add) throw new ArgumentException("An item with the same key has already been added.");
            if (Equals(item, value)) return;
            Dictionary[key] = value;

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
         }
         else
         {
            Dictionary[key] = value;

            OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
         }
      }
      /// <summary>
      /// 
      /// </summary>
      private void OnPropertyChanged()
      {
         OnPropertyChanged(CountString);
         OnPropertyChanged(IndexerName);
         OnPropertyChanged(KeysName);
         OnPropertyChanged(ValuesName);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="propertyName"></param>
      protected virtual void OnPropertyChanged(string propertyName)
      {
         if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
      /// <summary>
      /// 
      /// </summary>
      private void OnCollectionChanged()
      {
         if (this.enableNotifications)
         {
            OnPropertyChanged();
            {
               if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="action"></param>
      /// <param name="changedItem"></param>
      private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
      {
         if (this.enableNotifications)
         {
            OnPropertyChanged();

            if (CollectionChanged != null)
            {
               CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="action"></param>
      /// <param name="newItem"></param>
      /// <param name="oldItem"></param>
      private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
      {
         if (this.enableNotifications)
         {
            OnPropertyChanged();

            if (CollectionChanged != null)
            {
               CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="action"></param>
      /// <param name="newItems"></param>
      private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
      {
         if (this.enableNotifications)
         {
            OnPropertyChanged();

            if (CollectionChanged != null)
            {
               CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
            }
         }
      }
   }
}
