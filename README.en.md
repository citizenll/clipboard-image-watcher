# Clipboard Image Watcher

English | [中文](README.md)

A lightweight Windows tray application that automatically monitors clipboard screenshots, saves memory images to local files, and replaces clipboard content with file references to reduce memory usage.

## ✨ Features

- **Automatic Screenshot Detection** - Real-time monitoring of memory images in clipboard (like screenshots)
- **Smart Memory Management** - Saves memory images as local PNG files and replaces clipboard content with file references
- **Cache Management** - Keeps maximum of 3 files, each preserved for up to 1 hour
- **Non-Intrusive Operation** - Pure tray application with no main window, doesn't interrupt workflow
- **Real-time Feedback** - Tray icon tooltip shows last capture time
- **Auto Cleanup** - Automatically deletes all cache files when program exits

## 🚀 Quick Start

### System Requirements

- Windows 10/11
- .NET 9.0

### Installation & Running

1. Download the latest executable file
2. Ensure `tray.ico` icon file is in the same directory
3. Double-click to run `ClipboardImageWatcher.exe`
4. The program will display an icon in the system tray

### Usage

1. **Start Application** - After running, an icon will appear in the system tray
2. **Take Screenshots** - Use any screenshot tool (like Windows + Shift + S)
3. **Automatic Processing** - App automatically detects clipboard images and saves them locally
4. **Paste Usage** - When pasting, local files are used instead of memory images
5. **Exit Application** - Right-click tray icon and select "Exit"

## 🔧 How It Works

```
User Screenshots → Image enters clipboard memory
    ↓
App Detection → Monitors clipboard changes
    ↓
Save File → Saves memory image as local PNG
    ↓
Replace Clipboard → Replaces memory image with file reference
    ↓
User Paste → Pastes local file (reduces memory usage)
```

## 📋 Feature Details

### Cache Management
- **File Limit**: Maximum of 3 screenshot files
- **Time Limit**: Each file preserved for up to 1 hour
- **Auto Cleanup**: Checks and deletes expired files every 5 minutes
- **Exit Cleanup**: Deletes all cache files when program closes

### User Interface
- **Tray Icon**: Shows application running status
- **Tooltip**: Mouse hover displays last capture time
- **Context Menu**: Provides exit option
- **No Main Window**: Doesn't show in taskbar, doesn't interrupt work

## 🛠️ Development & Building

### Environment Requirements
- Visual Studio 2022 or higher
- .NET 9.0 SDK
- Windows SDK

### Build Steps

```bash
# Clone repository
git clone https://github.com/your-username/clipboard-image-watcher.git
cd clipboard-image-watcher

# Build project
dotnet build ClipboardImageWatcher/ClipboardImageWatcher.csproj

# Run project
dotnet run --project ClipboardImageWatcher/ClipboardImageWatcher.csproj
```

### Project Structure

```
cc-paste/
├── ClipboardImageWatcher/
│   ├── App.xaml              # WPF application entry point
│   ├── App.xaml.cs           # Application startup logic
│   ├── MainWindow.xaml       # Main window (hidden)
│   ├── MainWindow.xaml.cs    # Core functionality implementation
│   ├── ClipboardMonitor.cs   # Clipboard monitoring
│   ├── tray.ico              # Tray icon
│   └── ClipboardImageWatcher.csproj
├── .gitignore
├── README.md
├── README.en.md
└── cc-paste.sln
```

## 📝 Logging

The application creates a `log.txt` file in the program directory, recording:
- Application startup and shutdown
- Screenshot capture and processing
- File saving and cleanup
- Error information

## 🤝 Contributing

Issues and Pull Requests are welcome!

### Contribution Guidelines
1. Fork this repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🐛 Issue Reporting

If you encounter problems or have feature suggestions, please submit them on the [Issues](https://github.com/your-username/clipboard-image-watcher/issues) page.

## 📊 Version History

- **v1.0.0** - Initial release
  - Basic clipboard monitoring functionality
  - Memory image to file conversion
  - Automatic cache management
  - Tray icon interface

---

**Note**: This application only runs on Windows systems, using WPF and Windows Forms technology stack.