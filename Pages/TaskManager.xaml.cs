using CommunityToolkit.WinUI.UI.Controls;
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
            IReadOnlyList<CoreWebView2ProcessExtendedInfo> eInfos = await App.WebView2.CoreWebView2.Environment.GetProcessExtendedInfosAsync();
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

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            foreach (var column in dataGrid.Columns)
            {
                if (column != e.Column)
                {
                    column.SortDirection = null;
                }
            }

            if (e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                e.Column.SortDirection = DataGridSortDirection.Descending;
                dataGrid.ItemsSource = (string)e.Column.Tag switch
                {
                    "Task" => infos.OrderByDescending(x => x.Task),
                    "Memory" => infos.OrderByDescending(x => x.Memory),
                    "ProcessId" => infos.OrderByDescending(x => x.ProcessId),
                    _ => infos
                };
            }
            else
            {
                e.Column.SortDirection = DataGridSortDirection.Ascending;
                dataGrid.ItemsSource = (string)e.Column.Tag switch
                {
                    "Task" => infos.OrderBy(x => x.Task),
                    "Memory" => infos.OrderBy(x => x.Memory),
                    "ProcessId" => infos.OrderBy(x => x.ProcessId),
                    _ => infos
                };
            }
        }
    }
}
