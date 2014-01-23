using FreeSrc.DevUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FreeSrc.DevUtils.Commands
{
    public abstract class UiCommand<ViewModelT> : ICommand
        where ViewModelT : ViewModelSkeleton
    {
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual event EventHandler CanExecuteChanged { add { ;} remove { ;} }

        public string VMName { get; set; }

        public void Execute(object parameter)
        {
            UpdateViewModel(WindowVM.Instance[this.VMName] as ViewModelT);
        }

        protected abstract void UpdateViewModel(ViewModelT viewModel);
    }
}
