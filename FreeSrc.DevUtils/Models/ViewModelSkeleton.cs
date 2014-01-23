using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSrc.DevUtils.Models
{
    public class ViewModelSkeleton : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected Dictionary<string, object> _state;

        public ViewModelSkeleton()
        {
            _state = new Dictionary<string, object>();
        }

        protected PropT GetProperty<PropT>(string propName, Func<PropT> factory = null )
        {
            if ( !_state.ContainsKey(propName))
            {
                if (factory == null)
                {
                    _state[propName] = default(PropT);
                }
                else
                {
                    _state[propName] = factory();
                }
            }

            return (PropT) _state[propName];
        }

        protected void SetProperty<PropT>(string propName, PropT propValue)
        {
            if (!_state.ContainsKey(propName))
            {
                _state[propName] = default(PropT);
            }
            else
            {
                _state[propName] = propValue;
            }

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
