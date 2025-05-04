using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;


namespace Edge
{
    public class ProcessInfo
    {
        public string Task { get; set; }
        public long Memory { get; set; }
        public string MemoryKB { get; set; }
        public int ProcessId { get; set; }
    }

    public sealed partial class TaskManager : Window
    {
        public ObservableCollection<ProcessInfo> infos = [];

        public TaskManager()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("./Assets/icon.ico");
            SetTitleBar(TitleBar);
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            this.SetBackdrop();
            this.SetThemeColor();

            InitialData();
        }

        private async void InitialData()
        {
            IReadOnlyList<CoreWebView2ProcessExtendedInfo> eInfos = await App.CoreWebView2Environment.GetProcessExtendedInfosAsync();
            foreach (CoreWebView2ProcessExtendedInfo item in eInfos)
            {
                ProcessInfo info = new()
                {
                    Task = item.ProcessInfo.Kind switch
                    {
                        CoreWebView2ProcessKind.Browser => "浏览器",
                        CoreWebView2ProcessKind.Renderer => item.AssociatedFrameInfos[0].Source,
                        CoreWebView2ProcessKind.Utility => "实用工具",
                        CoreWebView2ProcessKind.Gpu => "GPU 进程",
                        _ => "未知"
                    },
                    ProcessId = item.ProcessInfo.ProcessId
                };

                Process process = Process.GetProcessById(item.ProcessInfo.ProcessId);
                info.Memory = process.PrivateMemorySize64 / 1024;
                info.MemoryKB = $"{info.Memory} KB";
                infos.Add(info);
            }
        }
    }
}
