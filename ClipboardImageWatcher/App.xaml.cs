using System.Configuration;
using System.Data;
using System.Windows;

namespace ClipboardImageWatcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        var mainWindow = new MainWindow();
        // Don't show the window, just keep it for the tray functionality
        mainWindow.WindowState = WindowState.Minimized;
        mainWindow.ShowInTaskbar = false;
    }
}

