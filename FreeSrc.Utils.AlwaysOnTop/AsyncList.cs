using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FreeSrc.Utils.AlwaysOnTop
{
    public class AsyncList<ItemT> : ObservableCollection<ItemT>
    {
        private object _lock;

        public AsyncList()
        {
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(this, _lock);
        }
    }
}
