using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FreeSrc.DevUtils.Models
{
    public class AsyncCollection<ItemT> : ObservableCollection<ItemT>
    {
        protected object _lock = new object();

        public AsyncCollection()
        {
            BindingOperations.EnableCollectionSynchronization(this, _lock);
        }
    }
}
