using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSrc.DevUtils.Models
{
    public class WindowVM : ViewModelSkeleton
    {
        private static Dictionary<string, Type> ChildViewModels = new Dictionary<string, Type>()
        {
            { "MQViewer", typeof(MQViewerVM) },
        };

        public static WindowVM Instance { get; protected set; }

        public WindowVM()
        {
            WindowVM.Instance = this;

            foreach (var vmName in ChildViewModels.Keys)
            {
                var vmType = ChildViewModels[vmName];

                this.GetProperty<ViewModelSkeleton>(vmName, () => (ViewModelSkeleton) Activator.CreateInstance(vmType)); 
            }
        }

        public ViewModelSkeleton this[string vmName]
        {
            get
            {
                var vm =  this.GetProperty<ViewModelSkeleton>(vmName);

                return vm;
            }
        }
    }
}
