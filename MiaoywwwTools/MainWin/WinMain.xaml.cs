using MiaoywwwTools.Tools.WallPaper;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        public BackGround backGround;

        public WinMain()
        {
            InitializeComponent();
            winMain = this;
            WlpChangeSettings();
            backGround = new();
        }

        public bool WlpChangeSettings()
        {
            string wlpKeypath = @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\Tools\WallPaper";

            Settings.UseVideo = bool.Parse(Registry.GetValue(wlpKeypath, "VideoSettings_UseVideo", "False").ToString());
            Settings.UseWord = bool.Parse(Registry.GetValue(wlpKeypath, "WordSettings_UseWord", null).ToString());

            Settings.VideoVolume = double.Parse(Registry.GetValue(wlpKeypath, "VideoSettings_VideoVolume", "100.0").ToString());
            Settings.VideoLoop = bool.Parse(Registry.GetValue(wlpKeypath, "VideoSettings_VideoLoop", "False").ToString());

            try
            {
                Settings.VideoUri = new Uri(Registry.GetValue(wlpKeypath, "VideoSettings_VideoFilePath", null).ToString());
            }
            catch (UriFormatException)
            {
                Settings.VideoUri = null;
            }

            string tboxDate1_Text = Registry.GetValue(wlpKeypath, "WordSettings_Date1", null).ToString();
            string tboxDate2_Text = Registry.GetValue(wlpKeypath, "WordSettings_Date2", null).ToString();
            Settings.WordContent = Registry.GetValue(wlpKeypath, "WordSettings_WordContent", "").ToString();
            Settings.FontSize = double.Parse(Registry.GetValue(wlpKeypath, "WordSettings_FontSize", null).ToString());
            try
            {
                if (tboxDate1_Text != "Now")
                {
                    DateTime dt1 = Convert.ToDateTime(tboxDate1_Text);
                }
                if (tboxDate2_Text != "Now")
                {
                    DateTime dt2 = Convert.ToDateTime(tboxDate2_Text);
                }
                Settings.WordDate1 = tboxDate1_Text;
                Settings.WordDate2 = tboxDate2_Text;
            }
            catch (FormatException)
            {
                MessageBox.ShowDialog("请输入一个正确的日期,如2022-6-14");
                return false;
            } 

            BrushConverter brushConverter = new BrushConverter();
            try
            {
                Settings.WordColor = (Brush)brushConverter.ConvertFromString(Registry.GetValue(wlpKeypath, "WordSettings_WordColor", null).ToString());
            }
            catch (Exception)
            {
                Settings.WordColor = (Brush)brushConverter.ConvertFromString("#FFFFFFFF");
                Registry.SetValue(wlpKeypath, "WordSettings_WordColor", "#FFFFFFFF");
            }
            return true;
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
            if (nowpage + 1 > maxpage)
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