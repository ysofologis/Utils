using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FreeSrc.Utils.AlwaysOnTop
{
    public class DesktopWindow 
    {
        private bool _alwaysOnTop;

        public DesktopWindow()
        {
        }

        public IntPtr hWnd { get; set; }
        public string Title { get; set; }
        public BitmapImage Icon { get; set; }
        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop; }

            set { 
                _alwaysOnTop = value;
                SetAlwaysOnTop();
            }
        }

        protected void SetAlwaysOnTop()
        {
            if (this.AlwaysOnTop)
            {
                Win32Api.SetWindowPos(this.hWnd, Win32Api.HWND.TopMost, -1, -1, -1, -1, Win32Api.SetWindowPosFlags.SHOWWINDOW | Win32Api.SetWindowPosFlags.NOMOVE | Win32Api.SetWindowPosFlags.NOSIZE);
            }
            else
            {
                Win32Api.SetWindowPos(this.hWnd, Win32Api.HWND.NoTopMost, 0, 0, 0, 0, Win32Api.SetWindowPosFlags.SHOWWINDOW | Win32Api.SetWindowPosFlags.NOMOVE | Win32Api.SetWindowPosFlags.NOSIZE);
            }
        }
    }
}
