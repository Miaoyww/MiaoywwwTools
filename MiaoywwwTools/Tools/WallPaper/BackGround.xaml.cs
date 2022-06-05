using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using static MiaoywwwTools.Tools.WallPaper.MainWallPaper;

namespace MiaoywwwTools.Tools.WallPaper
{
    /// <summary>
    /// BackGround.xaml 的交互逻辑
    /// </summary>
    public partial class BackGround : Window
    {
        private IntPtr programHandle;
        private DispatcherTimer timerTick = new DispatcherTimer();
        private DispatcherTimer timerGetSettings = new DispatcherTimer();

        public BackGround()
        {
            InitializeComponent();
            SendMsgToProgman();
            ChangeWord();
            ChangeVideo(false);
            timerTick.Tick += new EventHandler(timerTick_Tick);
            timerTick.Interval = TimeSpan.FromSeconds(10); //设置刷新的间隔时间
            timerTick.Start();
        }

        public void ChangeWord()
        {
            if ((bool)Settings.UseWord)
            {
                labContent.Foreground = Settings.WordColor;
                Tick();
            }
            else
            {
                    labContent.Content = "";
            }
        }

        public void ChangeVideo(bool play)
        {
            if ((bool)Settings.UseVideo)
            {
                medMain.Source = Settings.VideoUri;
                medMain.Visibility = Visibility.Visible;
                medMain.Volume = (double)Settings.VideoVolume / 100;
                if (play)
                {
                    medMain.Play();
                }
            }
        }

        private void medMain_MediaEnded(object sender, RoutedEventArgs e)
        {
            if ((bool)Settings.VideoLoop)
            {
                medMain.Position = TimeSpan.Zero;
                medMain.Play();

            }
            else
            {
                medMain.Visibility = Visibility.Hidden;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Win32Func.SetParent(new WindowInteropHelper(this).Handle, programHandle);
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = 0;
            this.Top = 0;
            GetSettings();
            timerGetSettings.Tick += new EventHandler(timerGetSettings_Tick);
            timerGetSettings.Interval = TimeSpan.FromSeconds(0.5); //设置刷新的间隔时间
            timerGetSettings.Start();

        }

        private void Tick()
        {
            DateTime dt1;
            DateTime dt2;
            if (Settings.WordDate1 == "Now")
            {
                dt1 = Convert.ToDateTime(DateTime.Now);
            }
            else
            {
                dt1 = Convert.ToDateTime(Settings.WordDate1);
            }
            if (Settings.WordDate2 == "Now")
            {
                dt2 = Convert.ToDateTime(DateTime.Now);
            }
            else
            {
                dt2 = Convert.ToDateTime(Settings.WordDate2);
            }
            TimeSpan timeSpan = dt1.Subtract(dt2);
            labContent.Content = string.Format($"{Settings.WordContent}", timeSpan.Days + 1);
        }

        private void GetSettings()
        {
            labContent.FontSize = (double)Settings.FontSize;
        }
        private void timerTick_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        private void timerGetSettings_Tick(object sender, EventArgs e)
        {
            GetSettings();
        }

        /// <summary>
        /// 向桌面发送消息
        /// </summary>
        public void SendMsgToProgman()
        {
            // 桌面窗口句柄，在外部定义，用于后面将我们自己的窗口作为子窗口放入
            programHandle = Win32Func.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;
            // 向 Program Manager 窗口发送消息 0x52c 的一个消息，超时设置为2秒
            Win32Func.SendMessageTimeout(programHandle, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 2, result);

            // 遍历顶级窗口
            Win32Func.EnumWindows((hwnd, lParam) =>
            {
                // 找到第一个 WorkerW 窗口，此窗口中有子窗口 SHELLDLL_DefView，所以先找子窗口
                if (Win32Func.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                {
                    // 找到当前第一个 WorkerW 窗口的，后一个窗口，及第二个 WorkerW 窗口。
                    IntPtr tempHwnd = Win32Func.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                    // 隐藏第二个 WorkerW 窗口
                    Win32Func.ShowWindow(tempHwnd, 0);
                }
                return true;
            }, IntPtr.Zero);
        }

        //Win32方法
        public static class Win32Func
        {
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string className, string winName);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlage, uint timeout, IntPtr result);

            //查找窗口的委托 查找逻辑
            public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string winName);

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parentHwnd);
        }
    }
}