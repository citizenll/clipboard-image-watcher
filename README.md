# 剪贴板图像监视器 (Clipboard Image Watcher)

[English](README.en.md) | 中文

一个轻量级的 Windows 托盘应用程序，自动监控剪贴板中的截图，将内存图像保存到本地文件并用文件引用替换剪贴板内容，从而减少内存占用。

## ✨ 功能特点

- **自动截图检测** - 实时监控剪贴板中的内存图像（如屏幕截图）
- **智能内存管理** - 将内存中的图像保存为本地 PNG 文件，用文件引用替换剪贴板内容
- **缓存管理** - 最多保存 3 个文件，每个文件最长保存 1 小时
- **无干扰运行** - 纯托盘应用，无主窗口，不打断工作流程
- **实时反馈** - 托盘图标工具提示显示最后捕获时间
- **自动清理** - 程序退出时自动删除所有缓存文件

## 🚀 快速开始

### 系统要求

- Windows 10/11
- .NET 9.0

### 安装运行

1. 下载最新版本的可执行文件
2. 确保 `tray.ico` 图标文件在同一目录下
3. 双击运行 `ClipboardImageWatcher.exe`
4. 程序将在系统托盘中显示图标

### 使用方法

1. **启动应用** - 运行后会在系统托盘显示图标
2. **截图操作** - 使用任何截图工具（如 Windows + Shift + S）
3. **自动处理** - 应用自动检测剪贴板中的图像并保存到本地
4. **粘贴使用** - 粘贴时使用的是本地文件而非内存图像
5. **退出应用** - 右键托盘图标选择"Exit"

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

## 📊 版本历史

- **v1.0.0** - 初始版本
  - 基本的剪贴板监控功能
  - 内存图像到文件的转换
  - 自动缓存管理
  - 托盘图标界面

---

**注意**: 本应用仅在 Windows 系统上运行，使用 WPF 和 Windows Forms 技术栈。