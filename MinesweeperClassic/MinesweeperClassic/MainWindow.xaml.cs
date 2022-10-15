using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MinesweeperLibrary;
using Windows.Devices.Geolocation;
using Microsoft.UI;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MinesweeperClassic
{
    public sealed partial class MainWindow : Window
    {
        //Window Objects (Meant for pages to access)
        public static MainWindow CurrentInstance;

        //Game settings
        public static int Rows = 9;
        public static int Cols = 9;
        public static int Mines = 9;

        public MainWindow()
        {
            this.InitializeComponent();

            //Record the current instance of the window so, it can be accessed by pages
            CurrentInstance = this;

            //Set icon of window
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Logo.ico"));

            //Navigate to the Board Settings Page
            ContentFrame.Navigate(typeof(BoardSettingsPage));
        }
    }
}
