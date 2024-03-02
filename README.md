# WinUIEdge

一个简易浏览器，基于WinUI3 和 Edge WebView2

**注意：** 如果想要使用此软件，需要安装 [Microsoft Edge WebView2](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/) ，因为此软件的核心功能依赖于此。

## 特色功能

### 本地控件制作的新版 “主页”

![Home Page](Assets/ReadmeSource/home-page.png)

提供了搜索框以及下方的推荐网站，相较于国内浏览器的网站导航，整体 **干净清爽** 。

具有 **高自定义度** ，可以在设置中选择主页的壁纸，以及选择是否隐藏推荐的网站。

### 快捷打开Dev Tools 工具

在网页的右下角提供了快捷打开 DevTools 窗口的按钮

### 搜索栏临时切换搜索引擎

在搜索栏的左侧，提供可以切换搜索引擎的选项，切换后的此次搜索将采用指定的搜索引擎，默认搜索引擎不受影响

![Change Search Engine](Assets/ReadmeSource/change-search-engine.png)

### 深浅色自由切换

软件支持深浅色的自由切换，也可以跟随系统主题，同时支持 `Mica`, `Acrylic` 等多种系统主题

![App Theme](Assets/ReadmeSource/app-theme.png)

下载、历史记录的弹出窗口使用重绘的窗口，并不会采用浏览器自带的弹出窗口

### 使用原生组件渲染本地文本

在显示本地文件时，传统浏览器的呈现方式不是很好，在本软件中，采用 **原生组件** 渲染，并且在文件 **预览** 上，提供 **字体，字号的更改** ，以及 **切换文件编码格式**。

同时还能显示 `文件行尾序列` ， `文件类型` 等。

![Text File Preview](Assets/ReadmeSource/text-file-preview.png)

### 设置界面提供自定义选项

在设置界面，可以实现不少对浏览器的自定义控制。

![Settings Page](Assets/ReadmeSource/settings-page.png)

## 安装该软件的最低条件

1. 最新版本的 [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/)
2. 至少 Windows 11 21H2 (Build 22000)，后续将考虑添加 Win10 支持

## 如何从源代码构建

1. [Visual Studio 2022 Community](https://visualstudio.microsoft.com/zh-hans/vs/)
2. 确保安装 `.NET 桌面开发`， `通用 Windows 平台开发` 等组件
3. [.NET 8.0](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)
4. `NuGet` 包
