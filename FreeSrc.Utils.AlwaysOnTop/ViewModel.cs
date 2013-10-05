using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSrc.Utils.AlwaysOnTop
{
    public class ViewModel
    {
        public AsyncList<DesktopWindow> OpenedWindows { get; protected set; }

        public ViewModel()
        {
            this.OpenedWindows = new AsyncList<DesktopWindow>();
        }
    }
}
