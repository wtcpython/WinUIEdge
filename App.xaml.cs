using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Edge
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        //public static string ApplicationTheme = "Mica";
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--disable-features=msSmartScreenProtection");
            Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");

            m_window = new MainWindow();
            m_window.Activate();
        }

        public static double Dpi()
        {
            return GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(m_window)) / 96.0;
        }

        [DllImport("user32.dll")]
        static extern int GetDpiForWindow(IntPtr hwnd);

        public static Window Window { get { return m_window; } }
        private static Window m_window;
    }
}
