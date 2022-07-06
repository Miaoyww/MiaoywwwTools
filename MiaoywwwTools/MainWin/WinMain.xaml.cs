using MiaoywwwTools.Tools.WallPaper;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        private DispatcherTimer timerHitokoto = new DispatcherTimer();
        private bool visibility = true;
        private Transform btnHidVisTransform;

        public WinMain()
        {
            InitializeComponent();
            winMain = this;
            timerHitokoto.Tick += new EventHandler(timerGetHitokoto_Tick);
            timerHitokoto.Interval = TimeSpan.FromSeconds(10); //设置刷新的间隔时间
            timerHitokoto.Start();

            WlpChangeSettings();
            backGround = new();
            btnHidVisTransform = WinMain.winMain.Btns_HidVis.Background.RelativeTransform;
        }

        public JObject HttpGetJson(string url)
        {
            HttpClient httpClientV = new HttpClient();
            httpClientV.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            var downloadV = httpClientV.GetStringAsync(url).Result;

            JObject jsonContentV = (JObject)JsonConvert.DeserializeObject(downloadV);
            return jsonContentV;
        }

        public JObject HitokotoJson;
        public string HitokotoContent;
        public string HitokotoFrom;
        public string HitokotoFromWho;

        public void timerGetHitokoto_Tick(object sender, EventArgs e)
        {
            Thread thread = new(() =>
            {
                try
                {
                    string HitokotoType = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "Hitokoto", "random").ToString();
                    if (HitokotoType == "random")
                    {
                        HitokotoJson = HttpGetJson("https://v1.hitokoto.cn/");
                    }
                    else
                    {
                        HitokotoJson = HttpGetJson($"https://v1.hitokoto.cn/?c={HitokotoType}");
                    }

                    GlobalV.HitokotoContent = HitokotoJson["hitokoto"].ToString();
                    GlobalV.HitokotoFrom = HitokotoJson["from"].ToString();
                    GlobalV.HitokotoFromWho = HitokotoJson["from_who"].ToString();
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                        "HitokotoLastContent", GlobalV.HitokotoContent);
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                        "HitokotoLastFrom", GlobalV.HitokotoFrom);
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                        "HitokotoLastFromWho", GlobalV.HitokotoFromWho);
                }
                catch (Exception)
                {
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public bool WlpChangeSettings()
        {
            string wlpKeypath = @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\Tools\WallPaper";
            string[][] keylist = new string[13][];
            for (int i = 0; i < keylist.Length; i++)
            {
                keylist[i] = new string[2];
            }
            keylist[0] = new string[] { "StartOnBoot", "False" };
            keylist[1] = new string[] { "WordSettings_WordContent", "距离……还有{0}天" };
            keylist[2] = new string[] { "WordSettings_Date1", "2022-6-14" };
            keylist[3] = new string[] { "WordSettings_Date2", "Now" };
            keylist[4] = new string[] { "WordSettings_WordColor", "#FF000000" };
            keylist[5] = new string[] { "WordSettings_UseWord", "True" };
            keylist[6] = new string[] { "VideoSettings_UseVideo", "False" };
            keylist[7] = new string[] { "VideoSettings_VideoFilePath", "" };
            keylist[8] = new string[] { "VideoSettings_VideoVolume", "100.0" };
            keylist[9] = new string[] { "VideoSettings_VideoLoop", "True" };
            keylist[10] = new string[] { "VideoSettings_VideoName", "None" };
            keylist[11] = new string[] { "WordSettings_FontSize", "100" };
            keylist[12] = new string[] { "WordSettings_UseHitokoto", "False" };
            foreach (string[] item in keylist)
            {
                object? keyvalue = Registry.GetValue(wlpKeypath, item[0], null);
                if (keyvalue is null)
                {
                    Registry.SetValue(wlpKeypath, item[0], item[1]);
                }
            }
            Settings.UseVideo = bool.Parse(Registry.GetValue(wlpKeypath, "VideoSettings_UseVideo", "False").ToString());
            Settings.UseWord = bool.Parse(Registry.GetValue(wlpKeypath, "WordSettings_UseWord", null).ToString());
            Settings.UseHitokoto = bool.Parse(Registry.GetValue(wlpKeypath, "WordSettings_UseHitokoto", null).ToString());

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
        public static string HiOrVisPageName;

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
                HiOrVisPageName = pagename;
                visibility = true;
                /*
                                                     <ImageBrush.RelativeTransform>
                                        <TransformGroup>
                                            <ScaleTransform CenterY="0.5" CenterX="0.5" ScaleX="0.5" ScaleY="0.5" />
                                            <SkewTransform CenterX="0.5" CenterY="0.5" />
                                            <RotateTransform CenterX="0.5" CenterY="0.5" />
                                            <TranslateTransform X="0" />
                                        </TransformGroup>
                                    </ImageBrush.RelativeTransform>
                */

                Btns_HidVis.Background = new ImageBrush
                {
                    Stretch = Stretch.Uniform,
                    RelativeTransform = btnHidVisTransform,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/Images/icons/settings_arrow-left.png"))
                };
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
            /*
            if (GlobalV.WinMainHidden)
            {
                this.Visibility = Visibility.Hidden;

                backGround.medMain.Position = TimeSpan.Zero;
                backGround.ChangeVideo(true);
                backGround.ChangeWord();
                backGround.Show();
                GlobalV.Started = true;

                # 设置窗口为ToolWindow 用于隐藏ALT+TAB内显示
                WindowInteropHelper wndHelper = new(this);
                int exStyle = (int)SetWindowStyle.GetWindowLong(wndHelper.Handle, (int)SetWindowStyle.GetWindowLongFields.GWL_EXSTYLE);
                exStyle |= (int)SetWindowStyle.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
                SetWindowStyle.SetWindowLong(wndHelper.Handle, (int)SetWindowStyle.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
            }*/
        }

        private void Btn_Mini_Click(object sender, RoutedEventArgs e)
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate
                {
                    this.WindowState = WindowState.Minimized;
                    this.Visibility = Visibility.Hidden;
                };
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

        private void MenuItemShowOrHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
            this.Show();
            this.Activate();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void Btns_HidVis_Click(object sender, RoutedEventArgs e)
        {
            if (visibility)
            {
                visibility = false;
                NestPage.Content = null;
                HiOrVisPageName = PageName;
                PageName = null;
                Btns_HidVis.Background = new ImageBrush
                {
                    RelativeTransform = btnHidVisTransform,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/Images/icons/settings_arrow-right.png"))
                };
            }
            else
            {
                visibility = true;
                ChangePage(HiOrVisPageName);
            }
        }

        internal class NativeMethods
        {
            public const uint SWP_NOSIZE = 0x0001;
            public const uint SWP_NOMOVE = 0x0002;
            public const uint SWP_NOACTIVATE = 0x0010;
            public static readonly IntPtr HWND_BOTTOM = new(1);

            [DllImport("user32.dll")]
            internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);

            [DllImport("user32.dll")]
            internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, ShowDesktop.WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

            [DllImport("user32.dll")]
            internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern int GetClassName(IntPtr hwnd, StringBuilder name, int count);

            [DllImport("user32.dll")]
            internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        }

        public static class ShowDesktop
        {
            private const uint WINEVENT_OUTOFCONTEXT = 0u;
            private const uint EVENT_SYSTEM_FOREGROUND = 3u;
            public const uint SWP_NOSIZE = 0x0001;
            public const uint SWP_NOMOVE = 0x0002;
            public const uint SWP_NOACTIVATE = 0x0010;
            public static readonly IntPtr HWND_BOTTOM = new(1);
            public static readonly IntPtr HWND_TOP = new(0);

            private const string WORKERW = "WorkerW";
            private const string PROGMAN = "Progman";

            public static void AddHook(Window window)
            {
                if (IsHooked)
                {
                    return;
                }

                IsHooked = true;
                /**
                 * 已知BUG：
                 * Win10下使用一次徽标键+D不会触发EVENT_SYSTEM_FOREGROUND事件
                 * 需要使用2次后会一同触发2次EVENT_SYSTEM_FOREGROUND事件
                 * 其他系统下正常
                 * 相关：0x22窗体缩小事件
                 *       0x23窗体还原事件
                 */
                _delegate = new WinEventDelegate(WinEventHook);
                _hookIntPtr = NativeMethods.SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _delegate, 0, 0, WINEVENT_OUTOFCONTEXT);
                _window = window;
            }

            public static void RemoveHook()
            {
                if (!IsHooked)
                {
                    return;
                }

                IsHooked = false;

                NativeMethods.UnhookWinEvent(_hookIntPtr.Value);

                _delegate = null;
                _hookIntPtr = null;
                _window = null;
            }

            private static string GetWindowClass(IntPtr hwnd)
            {
                StringBuilder _sb = new(32);
                NativeMethods.GetClassName(hwnd, _sb, _sb.Capacity);
                return _sb.ToString();
            }

            internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

            private static void WinEventHook(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
            {
                if (eventType == EVENT_SYSTEM_FOREGROUND)
                {
                    string _class = GetWindowClass(hwnd);

                    if (string.Equals(_class, WORKERW, StringComparison.Ordinal)/* || string.Equals(_class, PROGMAN, StringComparison.Ordinal)*/ )
                    {
                        _window.Topmost = true;
                    }
                    else
                    {
                        _window.Topmost = false;
                    }
                }
            }

            public static bool IsHooked { get; private set; } = false;

            private static IntPtr? _hookIntPtr { get; set; }

            private static WinEventDelegate _delegate { get; set; }

            private static Window _window { get; set; }
        }

        internal class SetWindowStyle
        {
            [Flags]
            public enum ExtendedWindowStyles
            {
                WS_EX_TOOLWINDOW = 0x00000080,
            }

            public enum GetWindowLongFields
            {
                GWL_EXSTYLE = (-20),
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
            private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
            private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

            [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
            public static extern void SetLastError(int dwErrorCode);

            public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
            {
                int error = 0;
                IntPtr result = IntPtr.Zero;
                // Win32 SetWindowLong doesn't clear error on success
                SetLastError(0);

                if (IntPtr.Size == 4)
                {
                    // use SetWindowLong
                    Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                    error = Marshal.GetLastWin32Error();
                    result = new IntPtr(tempResult);
                }
                else
                {
                    // use SetWindowLongPtr
                    result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                    error = Marshal.GetLastWin32Error();
                }

                if ((result == IntPtr.Zero) && (error != 0))
                {
                    throw new System.ComponentModel.Win32Exception(error);
                }

                return result;
            }

            private static int IntPtrToInt32(IntPtr intPtr)
            {
                return unchecked((int)intPtr.ToInt64());
            }
        }
    }
}