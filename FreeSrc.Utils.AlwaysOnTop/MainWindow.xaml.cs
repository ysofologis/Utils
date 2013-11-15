using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FreeSrc.Utils.AlwaysOnTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, Win32Api.IEnumeratorCallback
    {
        private ViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new ViewModel();
            this.DataContext = _viewModel;

            scanDesktop_Click(null, null);
        }

        bool Win32Api.IEnumeratorCallback.WindowFound(IntPtr hWnd, int lParam)
        {
            var title = Win32Api.GetWindowText(hWnd);

            if (Win32Api.IsWindowVisible(hWnd) && ! string.IsNullOrWhiteSpace(title))
            {
                _viewModel.OpenedWindows.Add(new DesktopWindow() { hWnd = hWnd, Title = title, Icon = GetWindowIcon(hWnd) });
            }

            return true;
        }

        private BitmapImage GetWindowIcon(IntPtr hWnd)
        {
            var img = Win32Api.GetSmallWindowIcon(hWnd);
            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            MemoryStream memoryStream = new MemoryStream();
            img.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();

            return bitmap;
        }

        private void scanDesktop_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.BeginInvoke( new Action(  () =>
                        {
            _viewModel.OpenedWindows.Clear();

            Win32Api.GetDesktopWindowsTitles(this);
                        } ) );
                });
        }
    }
}
