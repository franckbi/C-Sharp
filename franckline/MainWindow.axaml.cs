using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DotNetBrowser.Browser;
using DotNetBrowser.Engine;
using DotNetBrowser.Navigation;
using DotNetBrowser.OffScreen;            // IOffScreenBrowser, etc.
using DotNetBrowser.OffScreen.Events;     // FrameReceivedEventArgs
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace franckline
{
    public partial class MainWindow : Window
    {
        private IEngine _engine;
        private IOffScreenBrowser _offScreenBrowser;

        private readonly List<string> _history = new()
        {
            "https://www.google.com",
            "https://github.com",
            "https://www.microsoft.com"
        };

        public MainWindow()
        {
            InitializeComponent(); // Hooks up the XAML controls

            // Populate ComboBox with initial URLs
            UrlComboBox.Items = _history;
            UrlComboBox.SelectedIndex = 0;

            // 1) Create an Engine (no need for RenderingMode in 3.x)
            //    Then create an off-screen browser
            var engineOptions = new EngineOptions.Builder().Build();
            _engine = EngineFactory.Create(engineOptions);

            // DotNetBrowser 3.x: specify BrowserType.OffScreen
            _offScreenBrowser = (IOffScreenBrowser)_engine.CreateBrowser(BrowserType.OffScreen);

            // 2) Listen for off-screen frames
            _offScreenBrowser.FrameReceived += OnFrameReceived;

            // 3) Load the initial URL
            _offScreenBrowser.Navigation.LoadUrl(_history[0]);
        }

        /// <summary>
        /// Fired on background thread whenever the off-screen browser has a new frame.
        /// We copy the frame to a WriteableBitmap, then display in the UI Image.
        /// </summary>
        private void OnFrameReceived(object? sender, FrameReceivedEventArgs e)
        {
            // Must marshal to UI thread
            Dispatcher.UIThread.Post(() =>
            {
                var frame = e.Frame;
                if (frame == null) return;

                int width = frame.Width;
                int height = frame.Height;
                byte[] pixels = frame.Pixels;

                if (width == 0 || height == 0 || pixels == null)
                    return;

                // Create an Avalonia WriteableBitmap in BGRA8888
                using var wb = new WriteableBitmap(
                    new PixelSize(width, height),
                    new Vector(96, 96),
                    Avalonia.Platform.PixelFormat.Bgra8888,
                    Avalonia.Platform.AlphaFormat.Unpremul);

                // Copy raw BGRA bytes into the bitmap
                using (var lockedBuffer = wb.Lock())
                {
                    Marshal.Copy(pixels, 0, lockedBuffer.Address, pixels.Length);
                }

                // Show it in the Image control
                BrowserImage.Source = wb.Clone();
            });
        }

        private void OnNavigateClick(object? sender, RoutedEventArgs e)
        {
            if (UrlComboBox.SelectedItem is string url && !string.IsNullOrWhiteSpace(url))
            {
                _offScreenBrowser.Navigation.LoadUrl(url);

                // Add new URL to history if it isn't there
                if (!_history.Contains(url))
                {
                    _history.Add(url);
                    UrlComboBox.Items = null;        // Force refresh
                    UrlComboBox.Items = _history;
                }
            }
        }

        private void OnGoBackClick(object? sender, RoutedEventArgs e)
        {
            if (_offScreenBrowser.Navigation.CanGoBack())
                _offScreenBrowser.Navigation.GoBack();
        }

        private void OnGoForwardClick(object? sender, RoutedEventArgs e)
        {
            if (_offScreenBrowser.Navigation.CanGoForward())
                _offScreenBrowser.Navigation.GoForward();
        }

        private void OnRefreshClick(object? sender, RoutedEventArgs e)
        {
            _offScreenBrowser.Navigation.Reload();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Cleanup DotNetBrowser resources
            _offScreenBrowser?.Dispose();
            _engine?.Dispose();
        }
    }
}
