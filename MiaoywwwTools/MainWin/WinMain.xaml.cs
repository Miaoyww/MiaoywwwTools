﻿using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinMain.xaml 的交互逻辑
    /// </summary>

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。

    public partial class WinMain : Window
    {
        public static WinMain? winMain;

        public WinMain()
        {
            InitializeComponent();
            winMain = this;
        }

        public static string PageName;     // 储存页面

        /// <summary>
        /// 切换窗口
        /// </summary>
        /// <param name="pagename"> 页面名称 </param>
        public void ChangePage(string pagename)
        {
            if (pagename != PageName)
            {
                Type pageType = Type.GetType(pagename);
                if (pageType != null)
                {
                    this.NestPage.Content = new Frame()
                    {
                        Content = Activator.CreateInstance(pageType)
                    };
                }
                PageName = pagename;
            }
        }

        // DragMove 窗口移动
        private void WindowMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (System.InvalidOperationException)
            {

            }

        }

        // 导航栏
        private void Btns_Home_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinHome");
        }

        private void Btns_More_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinMore");
        }

        private void Btns_Settings_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinSettings");
        }

        public void CloseWindow()
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { this.Close(); };
                story.Begin(this);
            }
        }

        // 关闭按钮，转到动画
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        // 开机主页
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinHome");
            string CheckUpdateOnStart = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "CheckUpdateOnStart", false).ToString();
            if (CheckUpdateOnStart is "True")
            {
                WinSettings check = new();
                check.CheckUpdate();
            }
        }

        private void Btn_Mini_Click(object sender, RoutedEventArgs e)
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { this.WindowState = WindowState.Minimized; };
                story.Begin(this);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GlobalV.AppRestart)
            {
                try
                {
                    ProcessStartInfo process = new ProcessStartInfo();
                    process.FileName = Environment.CurrentDirectory + @"\restart.exe";
                    process.Arguments = "MiaoywwwTools";
                    Process.Start(process);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    e.Cancel = true;
                    this.Visibility = Visibility.Hidden;
                    this.Opacity = 0;
                    MessageBox.ShowDialog("重启失败，请自行重启");
                    GlobalV.AppRestart = false;
                    CloseWindow();
                }
            }
        }

        private void Btns_next_Click(object sender, RoutedEventArgs e)
        {
            int maxpage = MainCarousel.Items.Count;
            int nowpage = MainCarousel.PageIndex;
            if(nowpage + 1 > maxpage)
            {
                MainCarousel.PageIndex = 0;
            }
            else
            {
                MainCarousel.PageIndex++;
            }
        }
    }
}