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

[English](README.en.md) | 中文

> 解决 Claude Code CLI 在 Windows 终端无法粘贴截图的问题

一个轻量级的 Windows 托盘应用程序，专门解决 Claude Code CLI 在 Windows 终端中无法粘贴内存截图的问题。自动将剪贴板中的内存图像转换为本地文件，让你可以在终端中正常粘贴截图。

## 🎯 解决的问题

**Claude Code CLI 无法粘贴截图？** 这是因为终端只能处理文件路径，无法直接粘贴内存中的图像数据。

**本应用的解决方案：**
- 📸 截图时自动检测剪贴板中的内存图像
- 💾 将内存图像保存为本地 PNG 文件
- 🔄 用文件路径替换剪贴板中的内存数据
- ✅ 现在可以在 Claude Code CLI 中正常粘贴截图了！

## ✨ 功能特点

- **🎯 专为 Claude Code 优化** - 完美解决终端粘贴截图问题
- **⚡ 自动处理** - 截图后立即转换，无需手动操作
- **🗂️ 智能缓存管理** - 最多保存 3 个文件，每个文件保存 1 小时
- **👻 隐形运行** - 纯托盘应用，不干扰正常工作
- **💡 实时反馈** - 鼠标悬停显示最后截图时间
- **🧹 自动清理** - 程序退出时清理所有临时文件

## 🚀 快速开始

### 系统要求

- Windows 10/11
- .NET 9.0

### 安装运行

1. 从 [Releases](https://github.com/your-username/clipboard-image-watcher/releases) 下载最新版本
2. 下载 `ClipboardImageWatcher.exe` 文件
3. 双击运行（托盘图标已内嵌，无需额外文件）
4. 程序将在系统托盘中显示图标

### 使用方法

1. **启动应用** - 双击运行，在系统托盘显示图标
2. **打开 Claude Code CLI** - 在终端中启动 Claude Code
3. **截图** - 使用 Windows + Shift + S 或其他截图工具
4. **粘贴到 Claude Code** - 在 Claude Code CLI 中按 Ctrl+V 粘贴
5. **✅ 成功！** - 现在可以正常在终端中粘贴和发送截图了

#### 💡 工作流程

```
截图 → 内存图像 → 自动转换 → 本地文件 → 终端可粘贴 ✅
```

**不使用本应用：**
```
截图 → 内存图像 → 粘贴到终端 → ❌ 失败（终端无法处理内存图像）
```

**使用本应用：**
```
截图 → 内存图像 → 自动保存为文件 → 粘贴文件路径 → ✅ 成功！
```

## 🔧 工作原理

```
用户截图 → 图像进入剪贴板内存
    ↓
应用检测 → 监听剪贴板变化
    ↓
保存文件 → 将内存图像保存为本地 PNG
    ↓
替换剪贴板 → 用文件引用替换内存图像
    ↓
用户粘贴 → 粘贴本地文件（减少内存占用）
```

## 📋 功能详情

### 缓存管理
- **文件数量限制**: 最多保存 3 个截图文件
- **时间限制**: 每个文件最长保存 1 小时
- **自动清理**: 每 5 分钟检查并删除过期文件
- **退出清理**: 程序关闭时删除所有缓存文件

### 用户界面
- **托盘图标**: 显示应用运行状态
- **工具提示**: 鼠标悬停显示最后捕获时间
- **右键菜单**: 提供退出选项
- **无主窗口**: 不在任务栏显示，不干扰工作

## 🛠️ 开发构建

### 环境要求
- Visual Studio 2022 或更高版本
- .NET 9.0 SDK
- Windows SDK

### 构建步骤

```bash
# 克隆仓库
git clone https://github.com/your-username/clipboard-image-watcher.git
cd clipboard-image-watcher

# 构建项目
dotnet build ClipboardImageWatcher/ClipboardImageWatcher.csproj

# 运行项目
dotnet run --project ClipboardImageWatcher/ClipboardImageWatcher.csproj
```

### 项目结构

```
cc-paste/
├── ClipboardImageWatcher/
│   ├── App.xaml              # WPF 应用程序入口
│   ├── App.xaml.cs           # 应用程序启动逻辑
│   ├── MainWindow.xaml       # 主窗口（隐藏）
│   ├── MainWindow.xaml.cs    # 核心功能实现
│   ├── ClipboardMonitor.cs   # 剪贴板监控
│   ├── tray.ico              # 托盘图标
│   └── ClipboardImageWatcher.csproj
├── .gitignore
├── README.md
├── README.en.md
└── cc-paste.sln
```

## 📝 日志记录

应用会在程序目录下创建 `log.txt` 文件，记录以下信息：
- 应用启动和关闭
- 截图捕获和处理
- 文件保存和清理
- 错误信息

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

### 贡献指南
1. Fork 本仓库
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🐛 问题反馈

如果遇到问题或有功能建议，请在 [Issues](https://github.com/your-username/clipboard-image-watcher/issues) 页面提交。

## ❓ 常见问题

### Q: 为什么 Claude Code CLI 无法粘贴截图？
A: 因为终端应用程序只能处理文本和文件路径，无法直接处理剪贴板中的二进制图像数据。

### Q: 这个应用如何解决问题？
A: 它将剪贴板中的内存图像自动保存为本地PNG文件，然后用文件路径替换剪贴板内容，这样终端就可以识别和处理了。

### Q: 支持哪些截图工具？
A: 支持所有将图像保存到剪贴板的截图工具，包括：
- Windows + Shift + S（Windows 内置）
- Snipping Tool
- PicPick, Greenshot 等第三方工具

### Q: 会影响其他应用的粘贴功能吗？
A: 不会。当你粘贴到图像编辑器等应用时，系统会自动从文件加载图像，效果完全一样。

## 📊 版本历史

- **v1.0.0** - 初始版本
  - 专为 Claude Code CLI 优化
  - 自动截图转换功能
  - 智能缓存管理
  - 单文件发布

---

**专为 Claude Code 用户设计** | 仅支持 Windows 系统