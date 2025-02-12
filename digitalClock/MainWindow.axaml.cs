using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Timers;    

namespace digitalClock;

public partial class MainWindow : Window
{
  private readonly  Timer _t ;
    public MainWindow()
    {
        InitializeComponent();
        _t = new Timer(1000); // 1000ms = 1s interval
        _t.Elapsed += UpdateClock;
        _t.AutoReset = true;
        _t.Enabled = true;
        
        
    }
     private void UpdateClock(object? sender, ElapsedEventArgs e)
    {
        // Run UI updates on the main thread
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            ClockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
        });
    }
}