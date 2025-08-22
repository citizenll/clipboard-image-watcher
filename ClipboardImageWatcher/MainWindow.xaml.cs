using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
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
    private Timer? _processingTimer;
    private BitmapSource? _pendingImage;
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

    private bool IsMemoryImage()
    {
        try
        {
            // Check various clipboard formats that indicate file-based sources
            // FileDropList indicates files were copied from explorer
            if (System.Windows.Clipboard.ContainsFileDropList())
            {
                return false;
            }

            // Check for file-based clipboard formats
            var dataObject = System.Windows.Clipboard.GetDataObject();
            if (dataObject != null)
            {
                // Common file-based formats
                string[] fileFormats = {
                    "FileName",           // Single file name
                    "FileNameW",          // Unicode file name
                    "Shell IDList Array", // Shell object
                    "Preferred DropEffect" // File operation context
                };

                foreach (var format in fileFormats)
                {
                    if (dataObject.GetDataPresent(format))
                    {
                        Log($"Detected file-based clipboard format: {format}");
                        return false;
                    }
                }
            }

            Log("Clipboard contains memory-based image (likely screenshot)");
            return true;
        }
        catch (Exception ex)
        {
            Log($"Error checking clipboard format: {ex.Message}");
            // Default to processing if we can't determine the source
            return true;
        }
    }

    private void SaveImageWithCompatibility(BitmapSource image, string filePath)
    {
        Log($"Attempting to save image: {image.Format}, {image.PixelWidth}x{image.PixelHeight}, HasAlpha: {image.Format.ToString().Contains("A")}");
        
        // Analyze pixel data to detect transparent/blank images
        try
        {
            AnalyzeImagePixelData(image);
        }
        catch (Exception ex)
        {
            Log($"Pixel analysis failed: {ex.Message}");
        }
        
        try
        {
            // Method 1: Try direct clipboard data extraction for better quality
            if (TrySaveFromClipboardData(filePath))
            {
                Log($"Image saved successfully using direct clipboard data");
                return;
            }
        }
        catch (Exception ex)
        {
            Log($"Direct clipboard data save failed: {ex.Message}, trying standard encoding");
        }
        
        try
        {
            // Method 2: Try alternative clipboard format access
            if (TryAlternativeClipboardAccess(filePath))
            {
                Log($"Image saved successfully using alternative clipboard access");
                return;
            }
        }
        catch (Exception ex)
        {
            Log($"Alternative clipboard access failed: {ex.Message}, trying standard encoding");
        }
        
        try
        {
            // Method 3: Try standard PNG encoding
            var encoder = new PngBitmapEncoder();
            
            // Ensure image has proper format - convert if needed
            var convertedImage = ConvertToCompatibleFormat(image);
            encoder.Frames.Add(BitmapFrame.Create(convertedImage));
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(stream);
            }
            
            Log($"Image saved successfully using standard PNG encoder");
        }
        catch (Exception ex)
        {
            Log($"Standard PNG encoding failed: {ex.Message}, trying alternative method");
            
            try
            {
                // Method 3: Convert to bitmap and save using System.Drawing
                SaveImageUsingDrawing(image, filePath);
                Log($"Image saved successfully using System.Drawing fallback");
            }
            catch (Exception ex2)
            {
                Log($"All image saving methods failed: {ex2.Message}");
                throw;
            }
        }
    }

    private bool TryAlternativeClipboardAccess(string filePath)
    {
        try
        {
            var dataObject = System.Windows.Clipboard.GetDataObject();
            if (dataObject == null) return false;
            
            // Try System.Drawing.Bitmap format directly
            if (dataObject.GetDataPresent("System.Drawing.Bitmap"))
            {
                var drawingBitmap = dataObject.GetData("System.Drawing.Bitmap") as System.Drawing.Bitmap;
                if (drawingBitmap != null)
                {
                    Log($"Got System.Drawing.Bitmap: {drawingBitmap.Width}x{drawingBitmap.Height}, format: {drawingBitmap.PixelFormat}");
                    drawingBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    Log("Saved using System.Drawing.Bitmap format");
                    return true;
                }
            }
            
            // Try standard Bitmap format
            if (dataObject.GetDataPresent("Bitmap"))
            {
                var bitmapData = dataObject.GetData("Bitmap");
                Log($"Got Bitmap data type: {bitmapData?.GetType().Name}");
                
                if (bitmapData is System.Drawing.Bitmap drawingBmp)
                {
                    Log($"Converting Bitmap to System.Drawing.Bitmap: {drawingBmp.Width}x{drawingBmp.Height}");
                    drawingBmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    Log("Saved using converted Bitmap format");
                    return true;
                }
                else if (bitmapData is BitmapSource wpfBitmap)
                {
                    Log($"Converting WPF BitmapSource: {wpfBitmap.PixelWidth}x{wpfBitmap.PixelHeight}");
                    // Use our existing drawing method
                    SaveImageUsingDrawing(wpfBitmap, filePath);
                    Log("Saved using WPF BitmapSource conversion");
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Log($"Error in TryAlternativeClipboardAccess: {ex.Message}");
            return false;
        }
    }

    private bool TrySaveFromClipboardData(string filePath)
    {
        try
        {
            var dataObject = System.Windows.Clipboard.GetDataObject();
            if (dataObject == null) return false;
            
            // Try PNG format first (Snipaste might use this)
            if (dataObject.GetDataPresent("PNG"))
            {
                var pngData = dataObject.GetData("PNG") as Stream;
                if (pngData != null)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        pngData.CopyTo(fileStream);
                    }
                    Log("Saved using PNG clipboard format");
                    return true;
                }
            }
            
            // Try DIB (Device Independent Bitmap) format
            if (dataObject.GetDataPresent("DeviceIndependentBitmap") || dataObject.GetDataPresent("DIB"))
            {
                var formatName = dataObject.GetDataPresent("DeviceIndependentBitmap") ? "DeviceIndependentBitmap" : "DIB";
                var dibData = dataObject.GetData(formatName);
                if (dibData != null)
                {
                    Log($"Found {formatName} data, attempting to convert");
                    return SaveFromDIBData(dibData, filePath);
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Log($"Error in TrySaveFromClipboardData: {ex.Message}");
            return false;
        }
    }

    private bool SaveFromDIBData(object dibData, string filePath)
    {
        try
        {
            Log($"Processing DIB data type: {dibData.GetType().Name}");
            
            if (dibData is Stream stream)
            {
                Log($"DIB stream length: {stream.Length}, position: {stream.Position}");
                
                // Reset stream position
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                
                // Try creating bitmap with validation
                if (stream.Length > 0)
                {
                    using (var bitmap = new Bitmap(stream))
                    {
                        Log($"Created bitmap from DIB stream: {bitmap.Width}x{bitmap.Height}, format: {bitmap.PixelFormat}");
                        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        Log("Saved using DIB stream data");
                        return true;
                    }
                }
                else
                {
                    Log("DIB stream is empty");
                }
            }
            else if (dibData is byte[] bytes)
            {
                Log($"DIB byte array length: {bytes.Length}");
                
                if (bytes.Length > 0)
                {
                    using (var memoryStream = new MemoryStream(bytes))
                    {
                        using (var bitmap = new Bitmap(memoryStream))
                        {
                            Log($"Created bitmap from DIB bytes: {bitmap.Width}x{bitmap.Height}, format: {bitmap.PixelFormat}");
                            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                            Log("Saved using DIB byte array data");
                            return true;
                        }
                    }
                }
                else
                {
                    Log("DIB byte array is empty");
                }
            }
            else if (dibData is MemoryStream memStream)
            {
                Log($"DIB MemoryStream length: {memStream.Length}, position: {memStream.Position}");
                
                // Reset position
                memStream.Position = 0;
                
                if (memStream.Length > 0)
                {
                    using (var bitmap = new Bitmap(memStream))
                    {
                        Log($"Created bitmap from DIB MemoryStream: {bitmap.Width}x{bitmap.Height}, format: {bitmap.PixelFormat}");
                        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        Log("Saved using DIB MemoryStream data");
                        return true;
                    }
                }
                else
                {
                    Log("DIB MemoryStream is empty");
                }
            }
            
            Log($"DIB data type not supported: {dibData.GetType().Name}");
            return false;
        }
        catch (Exception ex)
        {
            Log($"Error processing DIB data: {ex.Message}");
            return false;
        }
    }

    private BitmapSource ConvertToCompatibleFormat(BitmapSource image)
    {
        try
        {
            // Check if image is already in a compatible format
            if (image.Format == System.Windows.Media.PixelFormats.Bgra32 || 
                image.Format == System.Windows.Media.PixelFormats.Bgr32 ||
                image.Format == System.Windows.Media.PixelFormats.Pbgra32)
            {
                return image;
            }

            // Convert to BGRA32 format for maximum compatibility
            var converter = new FormatConvertedBitmap();
            converter.BeginInit();
            converter.Source = image;
            converter.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            converter.EndInit();
            
            Log($"Converted image from {image.Format} to {converter.Format}");
            return converter;
        }
        catch (Exception ex)
        {
            Log($"Image format conversion failed: {ex.Message}, using original");
            return image;
        }
    }

    private void SaveImageUsingDrawing(BitmapSource bitmapSource, string filePath)
    {
        // Convert WPF BitmapSource to System.Drawing.Bitmap
        using (var memoryStream = new MemoryStream())
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            
            using (var bitmap = new Bitmap(memoryStream))
            {
                // Ensure bitmap has proper pixel format
                using (var compatibleBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (var graphics = Graphics.FromImage(compatibleBitmap))
                    {
                        graphics.DrawImage(bitmap, 0, 0);
                    }
                    
                    compatibleBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
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
        // Only process if it's an in-memory image (like a screenshot) and not from a file
        if (System.Windows.Clipboard.ContainsImage() && IsMemoryImage())
        {
            var image = System.Windows.Clipboard.GetImage();
            if (image != null)
            {
                Log($"Clipboard contains memory-based image - Format: {image.Format}, Size: {image.PixelWidth}x{image.PixelHeight}, DPI: {image.DpiX}x{image.DpiY}");
                AnalyzeClipboardFormats();
                
                // Store the image and delay processing to avoid interfering with screenshot tools
                _pendingImage = image;
                
                // Cancel any existing processing timer
                _processingTimer?.Stop();
                _processingTimer?.Dispose();
                
                // Create a new timer to process the image after a short delay
                _processingTimer = new Timer(500); // 500ms delay to let screenshot tools finish
                _processingTimer.Elapsed += ProcessPendingImage;
                _processingTimer.AutoReset = false;
                _processingTimer.Start();
                
                Log("Processing scheduled for 500ms delay to avoid interfering with screenshot tools");
            }
        }
        else if (System.Windows.Clipboard.ContainsImage())
        {
            Log("Clipboard contains image from file source, skipping processing");
        }
    }

    private void AnalyzeImagePixelData(BitmapSource image)
    {
        try
        {
            // Convert to a format we can analyze
            var convertedImage = new FormatConvertedBitmap(image, System.Windows.Media.PixelFormats.Bgra32, null, 0);
            
            // Sample some pixels to check if image is transparent/blank
            int stride = convertedImage.PixelWidth * 4; // 4 bytes per pixel (BGRA)
            byte[] pixels = new byte[stride * Math.Min(10, convertedImage.PixelHeight)]; // Sample first 10 rows
            
            convertedImage.CopyPixels(pixels, stride, 0);
            
            int opaquePixels = 0;
            int totalSampled = pixels.Length / 4;
            
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte alpha = pixels[i + 3]; // Alpha channel
                if (alpha > 0)
                {
                    opaquePixels++;
                }
            }
            
            double opacityRatio = (double)opaquePixels / totalSampled;
            Log($"Pixel analysis: {opaquePixels}/{totalSampled} opaque pixels (opacity ratio: {opacityRatio:F2})");
            
            if (opacityRatio < 0.01) // Less than 1% opaque pixels
            {
                Log("WARNING: Image appears to be mostly transparent - this may result in a blank saved file");
            }
        }
        catch (Exception ex)
        {
            Log($"Pixel data analysis error: {ex.Message}");
        }
    }

    private void AnalyzeClipboardFormats()
    {
        try
        {
            var dataObject = System.Windows.Clipboard.GetDataObject();
            if (dataObject != null)
            {
                var formats = dataObject.GetFormats();
                Log($"Available clipboard formats: {string.Join(", ", formats)}");
                
                // Check for specific Snipaste or image formats
                foreach (var format in formats)
                {
                    if (format.Contains("PNG") || format.Contains("Bitmap") || format.Contains("DIB") || format.Contains("CF_"))
                    {
                        Log($"Image-related format detected: {format}");
                        
                        try
                        {
                            var data = dataObject.GetData(format);
                            if (data != null)
                            {
                                Log($"Format {format} contains data type: {data.GetType().Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"Failed to read format {format}: {ex.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Error analyzing clipboard formats: {ex.Message}");
        }
    }

    private void ProcessPendingImage(object? sender, EventArgs e)
    {
        // Ensure we're on the UI thread for WPF objects
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => ProcessPendingImage(sender, e));
            return;
        }
        
        try
        {
            if (_pendingImage != null)
            {
                Log("Processing delayed image capture");
                
                CleanupAndMakeSpace();

                var filePath = Path.Combine(_storagePath, $"{FilePrefix}{DateTime.Now:yyyyMMddHHmmssfff}.png");
                SaveImageWithCompatibility(_pendingImage, filePath);

                // Replace clipboard memory image with file reference
                ReplaceClipboardWithFile(filePath);

                // Update tray icon tooltip with capture info
                UpdateTrayTooltip($"Last capture: {DateTime.Now:HH:mm:ss}");
                
                // Reset tooltip to default after 3 seconds
                ResetTooltipAfterDelay();
                Log($"Image saved to {filePath} and clipboard replaced with file reference");
                
                _pendingImage = null;
            }
            
            // Clean up timer
            _processingTimer?.Dispose();
            _processingTimer = null;
        }
        catch (Exception ex)
        {
            Log($"Error processing pending image: {ex.Message}");
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
        _processingTimer?.Stop();
        _processingTimer?.Dispose();
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
        _processingTimer?.Stop();
        _processingTimer?.Dispose();
        _notifyIcon?.Dispose();
        base.OnClosed(e);
    }
}