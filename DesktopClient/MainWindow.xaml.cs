using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Windows.Management.Deployment;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                var currentPackage = Windows.ApplicationModel.Package.Current;
                var version = currentPackage.Id.Version;

                VersionTextBlock.Text = $"{version.Major}.{version.Minor}.{version.Revision}.{version.Build}";
            }
            catch (InvalidOperationException)
            {

            }
        }

        private async void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filePicker = new OpenFileDialog { Filter = "MSIX bundle|*.msixbundle" };
            if (filePicker.ShowDialog() != true)
                return;
            
            var bundlePath = filePicker.FileName;
            var packageManager = new PackageManager();
            
            await packageManager.AddPackageByUriAsync(new Uri(bundlePath, UriKind.Absolute), new AddPackageOptions
            {
                DeferRegistrationWhenPackagesAreInUse = true,
                ForceTargetAppShutdown = false
            });

            StopButton.IsEnabled = true;
            ResultTextBlock.Text = "Ready to restart";
        }

        private async void InitWebView2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisposeAndClearOldWebView2();

                var webView = new WebView2 { Width = 400, Height = 200 };
                WebView2Root.Children.Add(webView);

                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Navigate("https://microsoft.com");
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = ex.ToString();
            }
        }

        private void DisposeAndClearOldWebView2()
        {
            foreach (var webView2 in WebView2Root.Children.OfType<WebView2>().ToList())
            {
                WebView2Root.Children.Remove(webView2);

                webView2.Dispose();
            }
        }

        private async void StopService_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "sc",
                Arguments = "stop TestWindowsService",
                UseShellExecute = true
            });

            await process.WaitForExitAsync();
            InitButton.IsEnabled = true;
        }
    }
}
