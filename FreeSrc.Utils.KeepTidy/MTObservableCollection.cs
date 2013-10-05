using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// 
   /// </summary>
   public class CollectionChangedWrapperEventData : IDisposable
   {
      #region Public Properties
      /// <summary>
      /// 
      /// </summary>
      public Dispatcher Dispatcher
      {
         get;
         set;
      }
      /// <summary>
      /// 
      /// </summary>
      public Action<NotifyCollectionChangedEventArgs> Action
      {
         get;
         set;
      }

      #endregion Public Properties

      #region Constructor
      /// <summary>
      /// 
      /// </summary>
      /// <param name="dispatcher"></param>
      /// <param name="action"></param>
      public CollectionChangedWrapperEventData(Dispatcher dispatcher, Action<NotifyCollectionChangedEventArgs> action)
      {
         Dispatcher = dispatcher;
         Action = action;
      }
      /// <summary>
      /// 
      /// </summary>
      ~CollectionChangedWrapperEventData()
      {
         this.Dispose();
      }

      #endregion Constructor
      /// <summary>
      /// 
      /// </summary>
      /// <param name="e"></param>
      public void executeEvent(NotifyCollectionChangedEventArgs e)
      {
         this.Action(e);
      }
      /// <summary>
      /// 
      /// </summary>
      public void Dispose()
      {
         this.Action = null;
         this.Dispatcher = null;
      }
   }
   /// <summary>
   /// Multithreaded observable collection.
   /// http://www.codeproject.com/Articles/64936/Multi-Threaded-ObservableCollection-and-NotifyColl
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class MTObservableCollection<T> : ObservableCollection<T>, IDisposable, IEnumerable<T>
      where T : class
   {
      /// <summary>
      /// 
      /// </summary>
      private Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> m_collectionChangedHandlers;
      /// <summary>
      /// 
      /// </summary>
      private object m_syncRoot;
      /// <summary>
      /// 
      /// </summary>
      private AutoResetEvent m_waitNotify;
      /// <summary>
      /// 
      /// </summary>
      public MTObservableCollection(DispatcherPriority dispatchPriority = DispatcherPriority.DataBind)
      {
         this.dispatchPriority = dispatchPriority;

         m_collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>();

         m_syncRoot = new object();

         m_waitNotify = new AutoResetEvent(false);
      }
      /// <summary>
      /// 
      /// </summary>
      public DispatcherPriority dispatchPriority
      {
         get;
         set;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="e"></param>
      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
         KeyValuePair<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData>[] handlers;

         lock (m_collectionChangedHandlers)
         {
            handlers = m_collectionChangedHandlers.ToArray();
         }

         if (handlers.Length > 0)
         {
            foreach (KeyValuePair<NotifyCollectionChangedEventHandler, CollectionChangedWrapperEventData> kvp in handlers)
            {
               if (kvp.Value.Dispatcher == null)
               {
                  if (kvp.Value.Action != null)
                  {
                     kvp.Value.executeEvent(e);
                  }
               }
               else
               {
                  if (kvp.Value.Action != null)
                  {
                     if (Dispatcher.CurrentDispatcher == kvp.Value.Dispatcher)
                     {
                        kvp.Value.Dispatcher.BeginInvoke((Action)delegate
                        {
                           kvp.Value.executeEvent(e);

                        }, this.dispatchPriority);
                     }
                     else
                     {
                        kvp.Value.Dispatcher.BeginInvoke((Action)delegate
                        {
                           kvp.Value.executeEvent(e);

                           m_waitNotify.Set();

                        }, this.dispatchPriority);

                        m_waitNotify.WaitOne();
                     }
                  }
               }
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public override event NotifyCollectionChangedEventHandler CollectionChanged
      {
         add
         {
            //Dispatcher dispatcher = Dispatcher.CurrentDispatcher; // should always work

            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread); // experimental (can return null)...

            /*
            if (dispatcher == null)
            {
               dispatcher = Application.Current.Dispatcher;
            }*/

            WeakRef<NotifyCollectionChangedEventHandler> weakEvent = new WeakRef<NotifyCollectionChangedEventHandler>(value);

            var action = new Action<NotifyCollectionChangedEventArgs>((args) =>
            {
               if (weakEvent.IsAlive)
               {
                  weakEvent.RefTarget(this, args);
               }
            });

            m_collectionChangedHandlers.Add(value, new CollectionChangedWrapperEventData(dispatcher, action));
         }
         remove
         {
            if (m_collectionChangedHandlers.ContainsKey(value))
            {
               m_collectionChangedHandlers.Remove(value);
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="index"></param>
      /// <param name="item"></param>
      protected override void InsertItem(int index, T item)
      {
         base.InsertItem(index, item);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="index"></param>
      protected override void RemoveItem(int index)
      {
         base.RemoveItem(index);
      }
      /// <summary>
      /// 
      /// </summary>
      public void Dispose()
      {
         foreach (var v in this)
         {
            if (v is IDisposable)
            {
               IDisposable d = v as IDisposable;

               d.Dispose();
            }
         }

         this.Clear();

         m_collectionChangedHandlers.Clear();

         m_waitNotify.Close();
      }
   }

}
