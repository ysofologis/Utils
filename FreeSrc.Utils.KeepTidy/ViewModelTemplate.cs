using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// 
   /// </summary>
   public class PropertyChangeSupport : INotifyPropertyChanged
   {
      /// <summary>
      /// 
      /// </summary>
      private List<PropertyChangedEventHandler> m_notifierList;
      /// <summary>
      /// 
      /// </summary>
      public PropertyChangeSupport()
      {
         m_notifierList = new List<PropertyChangedEventHandler>();
      }
      /// <summary>
      /// 
      /// </summary>
      public virtual string eventSource
      {
         get
         {
            return this.GetType().Name;
         }
      }

      #region INotifyPropertyChanged Members
      /// <summary>
      /// 
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged
      {
         add
         {
            lock (m_notifierList)
            {
               m_notifierList.Add(value);
            }
         }

         remove
         {
            lock (m_notifierList)
            {
               if (m_notifierList.Contains(value))
               {
                  m_notifierList.Remove(value);
               }
            }
         }
      }

      #endregion
      /// <summary>
      /// 
      /// </summary>
      /// <param name="propertyName"></param>
      public void broadcastPropertyChange(string propertyName)
      {
         PropertyChangedEventHandler[] listeners;

         lock (m_notifierList)
         {
            listeners = m_notifierList.ToArray();
         }

         foreach (PropertyChangedEventHandler h in listeners)
         {
            try
            {
               h(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception x)
            {
               this.writeLog(Log.Severity.Warn, x);

               AppState.traceException(x);
            }
         }

      }
      /// <summary>
      /// 
      /// </summary>
      protected void cleanupListeners()
      {
         lock (m_notifierList)
         {
            m_notifierList.Clear();
         }
      }
   }
   /// <summary>
   /// Base class template for custom 'ViewModel' construction.
   /// </summary>
   public abstract class TemplateVM : PropertyChangeSupport, IDataErrorInfo, IDisposable
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stateProperty"></param>
      /// <returns></returns>
      public delegate string ValidateCallback(IStateProperty stateProperty);
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stateProperty"></param>
      /// <returns></returns>
      public delegate void UpdateCallback(IStateProperty stateProperty, object oldValue, object newValue);
      /// <summary>
      /// 
      /// </summary>
      public interface IStateProperty : IDisposable
      {
         /// <summary>
         /// 
         /// </summary>
         string propertyName { get; }
         /// <summary>
         /// 
         /// </summary>
         object currentValue { get; }
         /// <summary>
         /// 
         /// </summary>
         object savedValue { get; }
         /// <summary>
         /// 
         /// </summary>
         bool hasBeenChanged { get; }
         /// <summary>
         /// 
         /// </summary>
         void setCurrentValue(object stateValue, bool notifyBinding);
         /// <summary>
         /// 
         /// </summary>
         void commitState();
         /// <summary>
         /// 
         /// </summary>
         void rollbackState();
         /// <summary>
         /// 
         /// </summary>
         ValidateCallback onValidate { get; }
         /// <summary>
         /// 
         /// </summary>
         UpdateCallback onUpdate { get; set; }
      }
      /// <summary>
      ///  
      /// </summary>
      public class StateProperty<StateType> : IStateProperty
      {
         /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>
         public delegate StateType CreateState();
         /// <summary>
         /// 
         /// </summary>
         protected WeakRef<TemplateVM> m_viewModel;
         /// <summary>
         /// 
         /// </summary>
         protected StateType m_currentValue;
         /// <summary>
         /// 
         /// </summary>
         protected StateType m_savedValue;
         /// <summary>
         /// 
         /// </summary>
         public string propertyName { get; set; }
         /// <summary>
         /// 
         /// </summary>
         public ValidateCallback onValidate { get; set; }
         /// <summary>
         /// 
         /// </summary>
         public UpdateCallback onUpdate { get; set; }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="viewModel"></param>
         /// <param name="propertyName"></param>
         /// <param name="stateValue"></param>
         public StateProperty(TemplateVM viewModel, string propertyName, StateType stateValue)
         {
            m_viewModel = new WeakRef<TemplateVM>(viewModel);
            this.propertyName = propertyName;

            m_savedValue = stateValue;
            m_currentValue = stateValue;

         }
         /// <summary>
         /// 
         /// </summary>
         public TemplateVM viewModel
         {
            get
            {
               return m_viewModel.RefTarget;
            }

            protected set
            {
               m_viewModel.RefTarget = value;
            }
         }

         /// <summary>
         /// 
         /// </summary>
         object IStateProperty.currentValue
         {
            get { return this.currentValue; }
         }
         /// <summary>
         /// 
         /// </summary>
         object IStateProperty.savedValue
         {
            get { return this.savedValue; }
         }
         /// <summary>
         /// 
         /// </summary>
         public StateType currentValue
         {
            get { return m_currentValue; }

            set
            {
               if (!compareValues(m_currentValue, value))
               {
                  var oldValue = m_currentValue;

                  m_currentValue = value;

                  if (this.onUpdate != null)
                  {
                     this.onUpdate(this, oldValue, value);
                  }

                  if (this.viewModel != null)
                  {
                     this.viewModel.notifyStateChanged(this.propertyName);
                  }
               }
            }
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="newValue"></param>
         public void setCurrentValue(object newValue, bool notifyBinding)
         {
            if (!(newValue is StateType))
            {
               throw new InvalidCastException("Value type in not accepted");
            }

            if (!compareValues(m_currentValue, (StateType)newValue))
            {
               var oldValue = newValue;

               m_currentValue = (StateType)newValue;

               if (this.onUpdate != null)
               {
                  this.onUpdate(this, oldValue, newValue);
               }

               if (notifyBinding)
               {
                  this.viewModel.notifyStateChanged(this.propertyName);
               }
            }
         }
         /// <summary>
         /// 
         /// </summary>
         public StateType savedValue
         {
            get { return m_savedValue; }
         }
         /// <summary>
         /// 
         /// </summary>
         public bool hasBeenChanged
         {
            get
            {
               return (!compareValues(m_currentValue, m_savedValue));
            }
         }
         /// <summary>
         /// 
         /// </summary>
         public bool isEmpty
         {
            get
            {
               return (m_currentValue == null) && (m_savedValue == null);
            }
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="l"></param>
         /// <param name="r"></param>
         /// <returns></returns>
         public static bool compareValues(StateType l, StateType r)
         {
            if (typeof(StateType).IsArray)
            {
               Array al = (Array)(object)l;
               Array ar = (Array)(object)r;

               if (object.ReferenceEquals(al, null) || object.ReferenceEquals(ar, null))
               {
                  return false;
               }

               if (al.Length != ar.Length)
               {
                  return false;
               }

               for (int ix = 0; ix < al.Length; ix++)
               {
                  if (!compareObjects(al.GetValue(ix), ar.GetValue(ix)))
                  {
                     return false;
                  }
               }

               return true;
            }
            else
            {
               return compareObjects(l, r);
            }
         }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="l"></param>
         /// <param name="r"></param>
         /// <returns></returns>
         public static bool compareObjects(object l, object r)
         {
            return object.Equals(l, r);
         }
         /// <summary>
         /// 
         /// </summary>
         public void commitState()
         {
            m_savedValue = m_currentValue;
         }
         /// <summary>
         /// 
         /// </summary>
         public void rollbackState()
         {
            m_currentValue = m_savedValue;

            this.viewModel.notifyStateChanged(this.propertyName);
         }
         /// <summary>
         /// 
         /// </summary>
         public void Dispose()
         {
            IDisposable crDisp = m_currentValue as IDisposable;
            IDisposable svDisp = m_savedValue as IDisposable;

            if (crDisp != null)
            {
               crDisp.Dispose();
            }

            if (svDisp != null)
            {
               svDisp.Dispose();
            }

            m_currentValue = default(StateType);
            m_savedValue = default(StateType);

            this.onUpdate = null;
            this.onValidate = null;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      private Dictionary<string, IStateProperty> m_stateMap;
      /// <summary>
      /// 
      /// </summary>
      protected object m_stateLock;
      /// <summary>
      /// 
      /// </summary>
      public TemplateVM()
      {
         m_stateLock = new object();

         m_stateMap = new Dictionary<string, IStateProperty>();

         this.stateValidationMessage = string.Empty;
      }
      /// <summary>
      /// 
      /// </summary>
      ~TemplateVM()
      {
      }
      /// <summary>
      /// 
      /// </summary>
      public TemplateVM self
      {
         get
         {
            return this;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      protected string stateValidationMessage { get; set; }
      /// <summary>
      /// 
      /// </summary>
      protected virtual object stateLock { get { return m_stateLock; } }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="propertyName"></param>
      /// <returns></returns>
      public IStateProperty this[string propertyName]
      {
         get
         {
            lock (this.stateLock)
            {
               if (m_stateMap.ContainsKey(propertyName))
               {
                  return m_stateMap[propertyName];
               }

               return null;
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public IStateProperty[] allProperties
      {
         get
         {
            lock (this.stateLock)
            {
               return m_stateMap.Values.ToArray();
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="propertyName"></param>
      public void notifyStateChanged(string propertyName)
      {
         broadcastPropertyChange(propertyName);
         broadcastPropertyChange("hasBeenChanged");
      }
      /// <summary>
      /// 
      /// </summary>
      public bool hasBeenChanged
      {
         get
         {
            lock (this.stateLock)
            {
               bool ret = false;

               foreach (IStateProperty p in m_stateMap.Values)
               {
                  ret |= p.hasBeenChanged;
               }

               return ret;
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public void commitState()
      {
         lock (this.stateLock)
         {
            foreach (IStateProperty p in m_stateMap.Values)
            {
               p.commitState();
            }
         }

         broadcastPropertyChange("hasBeenChanged");
      }
      /// <summary>
      /// 
      /// </summary>
      public void rollbackState()
      {
         lock (this.stateLock)
         {
            foreach (IStateProperty p in m_stateMap.Values)
            {
               p.rollbackState();
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public void Dispose()
      {
         cleanupState();
      }
      /// <summary>
      /// 
      /// </summary>
      private void cleanupState()
      {
         lock (this)
         {
            cleanupProperties();

            foreach (IStateProperty stateProp in m_stateMap.Values)
            {
               stateProp.Dispose();
            }

            m_stateMap.Clear();

            cleanupListeners();
         }
      }
      /// <summary>
      /// 
      /// </summary>
      protected abstract void cleanupProperties();
      /// <summary>
      /// 
      /// </summary>
      /// <param name="propertyName"></param>
      /// <returns></returns>
      protected StateProperty<StateType> createProperty<StateType>(string propertyName, StateProperty<StateType>.CreateState createState)
      {
         lock (this.stateLock)
         {
            if (!m_stateMap.ContainsKey(propertyName))
            {
               m_stateMap.Add(propertyName, new StateProperty<StateType>(this, propertyName, createState()));
            }

            return (StateProperty<StateType>)m_stateMap[propertyName];
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="StateType"></typeparam>
      /// <param name="propertyName"></param>
      /// <param name="defaultValue"></param>
      /// <returns></returns>
      protected StateProperty<StateType> createProperty<StateType>(string propertyName, StateType defaultValue = default(StateType))
      {
         lock (this.stateLock)
         {
            if (!m_stateMap.ContainsKey(propertyName))
            {
               m_stateMap.Add(propertyName, new StateProperty<StateType>(this, propertyName, defaultValue));
            }

            return (StateProperty<StateType>)m_stateMap[propertyName];
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="ValueT"></typeparam>
      /// <returns></returns>
      public ValueT[] getProperties<ValueT>()
      {
         lock (this)
         {
            List<ValueT> list = new List<ValueT>();

            foreach (object it in m_stateMap.Values)
            {
               if (it is ValueT)
               {
                  list.Add((ValueT)it);
               }
            }

            return list.ToArray();
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="ValueT"></typeparam>
      /// <param name="value"></param>
      public void updateProperties<ValueT>(ValueT value)
      {
         string[] keys = m_stateMap.Keys.ToArray();

         foreach (string key in keys)
         {
            if (m_stateMap[key] is ValueT)
            {
               createProperty<ValueT>(key).currentValue = value;
            }
         }
      }


      #region IDataErrorInfo Members
      /// <summary>
      /// 
      /// </summary>
      string IDataErrorInfo.Error
      {
         get
         {
            return this.stateValidationMessage;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="columnName"></param>
      /// <returns></returns>
      string IDataErrorInfo.this[string columnName]
      {
         get
         {
            lock (this)
            {
               if (m_stateMap.ContainsKey(columnName))
               {
                  IStateProperty stateProp = m_stateMap[columnName];

                  if (stateProp.onValidate != null)
                  {
                     return stateProp.onValidate(stateProp);
                  }
               }

               return string.Empty;
            }
         }
      }

      #endregion
   }
}
