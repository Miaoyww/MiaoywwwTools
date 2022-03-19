﻿using MiaoywwwTools.Tools.RandomDraw;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
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

        public Array times; // 预留接口，准备做ToolsRr的概率计算
        public ShowResult showResult = new();
        public bool Login;  // ToolsRr的Result窗口是否登录
        public bool FaceChanged; // Home的头像是否已经修改
        public bool CleanUpFace;    // 清除Home的头像
        public string PageName;     // 储存页面
        public int hwnd;
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
        private void Border_Top_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

        public void CloseWindow()
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Environment.Exit(0); };
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
            hwnd = (int)new WindowInteropHelper(this).Handle;
            ChangePage("MiaoywwwTools.WinHome");
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
    }
}