using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Reflection;
using Timer = System.Timers.Timer;

namespace ClipboardImageWatcher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int MaxFiles = 3;
    private const string FilePrefix = "capture_";
    private readonly string _storagePath = AppDomain.CurrentDomain.BaseDirectory;
    private NotifyIcon? _notifyIcon;
    private ClipboardMonitor? _clipboardMonitor;
    private Timer? _cleanupTimer;
    private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

    private void Log(string message)
    {
        try
        {
            File.AppendAllText(LogFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            // If logging fails, we can't do much, but let's at least write to debug output
            System.Diagnostics.Debug.WriteLine($"Failed to write to log: {ex.Message}");
        }
    }

    private Icon CreateIconFromPng(string pngPath)
    {
        try
        {
            using (var bitmap = new Bitmap(pngPath))
            {
                // Create a 16x16 icon from the PNG
                using (var resized = new Bitmap(bitmap, new System.Drawing.Size(16, 16)))
                {
                    IntPtr hIcon = resized.GetHicon();
                    return System.Drawing.Icon.FromHandle(hIcon);
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to create icon from PNG: {ex.Message}");
            // Return a default system icon as fallback
            return SystemIcons.Application;
        }
    }

    private void ReplaceClipboardWithFile(string filePath)
    {
        try
        {
            // Create a file drop list with our saved image
            var fileList = new System.Collections.Specialized.StringCollection();
            fileList.Add(filePath);

            // Clear clipboard and set file drop list
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetFileDropList(fileList);

            Log($"Clipboard replaced with file reference: {filePath}");
        }
        catch (Exception ex)
        {
            Log($"Failed to replace clipboard with file: {ex.Message}");
        }
    }

    private void UpdateTrayTooltip(string message)
    {
        try
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Text = $"Clipboard Image Watcher - {message}";
                Log($"Tray tooltip updated: {message}");
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to update tray tooltip: {ex.Message}");
        }
    }

    private void ResetTooltipAfterDelay()
    {
        var resetTimer = new Timer(3000); // 3 seconds
        resetTimer.Elapsed += (s, e) =>
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Text = "Clipboard Image Watcher";
                }
                resetTimer.Dispose();
            }
            catch (Exception ex)
            {
                Log($"Failed to reset tray tooltip: {ex.Message}");
            }
        };
        resetTimer.AutoReset = false;
        resetTimer.Start();
    }

    private Icon LoadEmbeddedIcon(string resourceName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullResourceName = $"ClipboardImageWatcher.{resourceName}";
            
            using (var stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream != null)
                {
                    return new Icon(stream);
                }
            }
            
            Log($"Embedded resource {fullResourceName} not found, using fallback");
            return SystemIcons.Application;
        }
        catch (Exception ex)
        {
            Log($"Failed to load embedded icon: {ex.Message}");
            return SystemIcons.Application;
        }
    }

    public MainWindow()
    {
        Log("Application starting.");

        try
        {
            Log("Creating NotifyIcon.");
            _notifyIcon = new NotifyIcon
            {
                Icon = LoadEmbeddedIcon("tray.ico"),
                Visible = true,
                Text = "Clipboard Image Watcher"
            };
            Log("NotifyIcon created successfully.");

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, OnExit);
            _notifyIcon.ContextMenuStrip = contextMenu;
            Log("Context menu created and assigned.");

            _clipboardMonitor = new ClipboardMonitor();
            _clipboardMonitor.ClipboardChanged += OnClipboardChanged;
            Log("Clipboard monitor initialized.");

            _cleanupTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            _cleanupTimer.Elapsed += (s, e) => CleanupOldFiles();
            _cleanupTimer.AutoReset = true;
            _cleanupTimer.Start();
            Log("Cleanup timer started.");

            // Initial cleanup
            CleanupOldFiles();
            Log("Initial cleanup complete. Application started.");
        }
        catch (Exception ex)
        {
            Log($"An error occurred during startup: {ex.Message}");
            Log(ex.ToString());
            // Optionally, show an error to the user if logging isn't enough
            System.Windows.MessageBox.Show($"An error occurred during startup. Please check the log file for details: {LogFilePath}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Windows.Application.Current.Shutdown();
        }
    }

    private void OnClipboardChanged(object? sender, EventArgs e)
    {
        // Only process if it's an in-memory image (like a screenshot) and not a file copy
        if (System.Windows.Clipboard.ContainsImage() && !System.Windows.Clipboard.ContainsFileDropList())
        {
            var image = System.Windows.Clipboard.GetImage();
            if (image != null)
            {
                CleanupAndMakeSpace();

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                var filePath = Path.Combine(_storagePath, $"{FilePrefix}{DateTime.Now:yyyyMMddHHmmssfff}.png");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                // Replace clipboard memory image with file reference
                ReplaceClipboardWithFile(filePath);

                // Update tray icon tooltip with capture info
                UpdateTrayTooltip($"Last capture: {DateTime.Now:HH:mm:ss}");
                
                // Reset tooltip to default after 3 seconds
                ResetTooltipAfterDelay();
                Log($"Image saved to {filePath} and clipboard replaced with file reference");
            }
        }
    }

    private void CleanupAndMakeSpace()
    {
        var files = GetCapturedFiles();
        if (files.Count() >= MaxFiles)
        {
            var oldestFile = files.First(); // Sorted by creation time ascending
            try
            {
                File.Delete(oldestFile.FullName);
            }
            catch (IOException ex)
            {
                // Log error or show a notification if needed
                System.Diagnostics.Debug.WriteLine($"Error deleting file: {ex.Message}");
            }
        }
    }

    private void CleanupOldFiles()
    {
        var files = GetCapturedFiles();
        var expirationTime = TimeSpan.FromHours(1);

        foreach (var file in files)
        {
            if (DateTime.Now - file.CreationTime > expirationTime)
            {
                try
                {
                    File.Delete(file.FullName);
                }
                catch (IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting old file: {ex.Message}");
                }
            }
        }
    }

    private IOrderedEnumerable<FileInfo> GetCapturedFiles()
    {
        var directory = new DirectoryInfo(_storagePath);
        return directory.GetFiles($"{FilePrefix}*.png")
                        .OrderBy(f => f.CreationTime);
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Log("Application shutting down - cleaning up all cached images");
        CleanupAllCachedImages();

        _cleanupTimer?.Stop();
        _cleanupTimer?.Dispose();
        _notifyIcon?.Dispose();
        System.Windows.Application.Current.Shutdown();
    }

    private void CleanupAllCachedImages()
    {
        try
        {
            var files = GetCapturedFiles();
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file.FullName);
                    Log($"Deleted cached image: {file.Name}");
                }
                catch (IOException ex)
                {
                    Log($"Failed to delete cached image {file.Name}: {ex.Message}");
                }
            }
            Log("All cached images cleanup completed");
        }
        catch (Exception ex)
        {
            Log($"Error during cleanup: {ex.Message}");
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        Log("Window closing - cleaning up resources");
        CleanupAllCachedImages();

        _cleanupTimer?.Stop();
        _cleanupTimer?.Dispose();
        _notifyIcon?.Dispose();
        base.OnClosed(e);
    }
}