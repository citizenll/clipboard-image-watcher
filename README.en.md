# 📋 Clipboard Image Watcher

<div style="background: #1a1a1a; border-radius: 8px; padding: 20px; margin: 20px 0; font-family: 'Cascadia Code', 'JetBrains Mono', 'Fira Code', Monaco, Consolas, monospace; color: #e6e6e6; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);">
  <div style="background: #2d2d30; padding: 10px; border-radius: 6px 6px 0 0; border: 1px solid #404040; display: flex; align-items: center; gap: 8px; font-size: 11px; color: #888;">
    <div style="color: #f97316;">🔥</div>
    <span>Welcome to Claude Code!</span>
    <div style="margin-left: auto; color: #666; font-size: 10px;">
      Enter to confirm · Esc to exit
    </div>
  </div>
  <div style="padding: 15px; background: #1a1a1a; border-radius: 0 0 6px 6px; border: 1px solid #404040; border-top: none;">
    <div style="color: #888; font-size: 11px; margin-bottom: 15px;">
      /help for help, /status for your current setup<br>
      <br>
      cwd: C:\Users\yourname
    </div>
    <div style="border-top: 1px solid #333; margin: 15px 0;"></div>
    <div style="display: flex; align-items: center; margin-top: 20px;">
      <span style="color: #4a9eff; margin-right: 8px;">&gt;</span>
      <span style="color: #f0f0f0; background: #2d2d30; padding: 2px 6px; border-radius: 3px;">[Image #1]</span>
      <span style="color: #888; margin-left: 5px; animation: blink 1s infinite;">▊</span>
    </div>
  </div>
</div>
<style>
@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}
</style>

English | [中文](README.md)

> Solve the issue where Claude Code CLI cannot paste screenshots in Windows terminal

A lightweight Windows tray application designed to solve the problem of Claude Code CLI not being able to paste memory screenshots in Windows terminal. Automatically converts clipboard memory images to local files, enabling normal screenshot pasting in terminal.

## 🎯 Problem Solved

**Can't paste screenshots in Claude Code CLI?** This happens because terminals can only handle file paths, not binary image data from memory.

**Our Solution:**
- 📸 Automatically detects memory images in clipboard when you take screenshots
- 💾 Saves memory images as local PNG files
- 🔄 Replaces clipboard memory data with file paths
- ✅ Now you can paste screenshots in Claude Code CLI normally!

## ✨ Features

- **🎯 Optimized for Claude Code** - Perfectly solves terminal screenshot pasting issue
- **⚡ Automatic Processing** - Converts screenshots instantly, no manual action needed
- **🗂️ Smart Cache Management** - Keeps max 3 files, each for 1 hour
- **👻 Invisible Operation** - Pure tray app that doesn't disturb your workflow
- **💡 Real-time Feedback** - Mouse hover shows last screenshot time
- **🧹 Auto Cleanup** - Cleans all temp files when program exits

## 🚀 Quick Start

### System Requirements

- Windows 10/11
- .NET 9.0

### Installation & Running

1. Download from [Releases](https://github.com/your-username/clipboard-image-watcher/releases)
2. Download `ClipboardImageWatcher.exe` file  
3. Double-click to run (icon is embedded, no additional files needed)
4. The program will display an icon in the system tray

### Usage

#### 🎬 Typical Use Case: Claude Code CLI

<div style="background: #1a1a1a; border-radius: 8px; padding: 20px; margin: 20px 0; font-family: 'Cascadia Code', 'JetBrains Mono', 'Fira Code', Monaco, Consolas, monospace; color: #e6e6e6; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);">
  <div style="background: #2d2d30; padding: 10px; border-radius: 6px 6px 0 0; border: 1px solid #404040; display: flex; align-items: center; gap: 8px; font-size: 11px; color: #888;">
    <div style="color: #f97316;">🔥</div>
    <span>Welcome to Claude Code!</span>
    <div style="margin-left: auto; color: #666; font-size: 10px;">
      Enter to confirm · Esc to exit
    </div>
  </div>
  
  <div style="padding: 15px; background: #1a1a1a; border-radius: 0 0 6px 6px; border: 1px solid #404040; border-top: none;">
    <div style="color: #888; font-size: 11px; margin-bottom: 15px;">
      /help for help, /status for your current setup<br>
      <br>
      cwd: C:\Users\citizenl
    </div>
    
    <div style="border-top: 1px solid #333; margin: 15px 0;"></div>
    
    <div style="color: #888; margin-bottom: 15px;">
      <strong>Overrides (via env):</strong><br>
      • API Base URL: http://127.0.0.1:6500
    </div>
    
    <div style="display: flex; align-items: center; margin-top: 20px;">
      <span style="color: #4a9eff; margin-right: 8px;">&gt;</span>
      <span style="color: #f0f0f0; background: #2d2d30; padding: 2px 6px; border-radius: 3px;">[Image #1]</span>
      <span style="color: #888; margin-left: 5px; animation: blink 1s infinite;">▊</span>
    </div>
  </div>
</div>

<style>
@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}
</style>

1. **Start App** - Double-click to run, tray icon appears
2. **Open Claude Code CLI** - Start Claude Code in your terminal
3. **Take Screenshot** - Use Windows + Shift + S or any screenshot tool
4. **Paste to Claude Code** - Press Ctrl+V in Claude Code CLI
5. **✅ Success!** - Now you can paste and send screenshots in terminal normally

#### 💡 Workflow

```
Screenshot → Memory Image → Auto Convert → Local File → Terminal Ready ✅
```

**Without this app:**
```
Screenshot → Memory Image → Paste to Terminal → ❌ Fail (Terminal can't handle memory images)
```

**With this app:**
```
Screenshot → Memory Image → Auto Save as File → Paste File Path → ✅ Success!
```

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

## ❓ FAQ

### Q: Why can't Claude Code CLI paste screenshots?
A: Because terminal applications can only handle text and file paths, not binary image data from clipboard memory.

### Q: How does this app solve the problem?
A: It automatically saves clipboard memory images as local PNG files, then replaces clipboard content with file paths that terminals can recognize and process.

### Q: Which screenshot tools are supported?
A: All screenshot tools that save images to clipboard, including:
- Windows + Shift + S (Windows built-in)
- Snipping Tool
- Third-party tools like PicPick, Greenshot, etc.

### Q: Will it affect pasting in other applications?
A: No. When you paste to image editors or other apps, the system automatically loads the image from the file, with identical results.

## 📊 Version History

- **v1.0.0** - Initial release
  - Optimized for Claude Code CLI
  - Automatic screenshot conversion
  - Smart cache management  
  - Single-file deployment

---

**Designed for Claude Code Users** | Windows Only