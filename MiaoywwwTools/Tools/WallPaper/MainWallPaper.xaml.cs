using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiaoywwwTools.Tools.WallPaper
{
    /// <summary>
    /// WallPaper.xaml 的交互逻辑
    /// </summary>
    public partial class MainWallPaper : Page
    {
        public MainWallPaper()
        {
            InitializeComponent();
        }

        public int wallpaperStyle; // 壁纸的对齐方式，也就是设置中的"契合度" * 失败
        /*
         * 0,居中
         * 1,平铺
         * 2,拉伸
         * 3,适应
         * 4,填充
         */
        public string selectpath;   // 用户所选择的壁纸路径
        public string wallpaperpath = System.Environment.CurrentDirectory + @"\Resources\Images\wallpaper\";   // 程序的壁纸路径
        public string selfkeypath = @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\Tools\WallPaper\";
        public string systemWallpaperConfig = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
        public string bingWallpaperApi = @"https://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

        private void Btn_App_Click(object sender, RoutedEventArgs e)
        {
            // 选择文件
            Microsoft.Win32.OpenFileDialog dialog = new();
            dialog.Title = "选择壁纸";
            dialog.DefaultExt = ".png";
            dialog.Filter = @"图像文件(*.jpg,*.png)|*jpeg;*.jpg;*.png;|JPEG(*.jpeg, *.jpg)|*.jpeg;*.jpg|PNG(*.png)|*.png";
            // 打开选择框选择
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                selectpath = dialog.FileName; // 获取选择的文件名
                try
                {
                    Registry.SetValue(systemWallpaperConfig, "WallPaperStyle", wallpaperStyle);
                }
                catch (UnauthorizedAccessException)
                {
                    WinMessage winMessage = new();
                    winMessage.SetMessage("错误", "请使用管理员模式再试", "close", "yesno");
                    winMessage.ShowDialog();
                    return;
                }
                Thread thread = new(() => SystemParametersInfo(20, 0, selectpath, wallpaperStyle));
                thread.Start();
            }
        }

        /*
         * 6、设置视窗的大小，SystemParametersInfo(6, 放大缩小值, P, 0)，lpvParam为long型
         * 17、开关屏保程序，SystemParametersInfo(17, False, P, 1)，uParam为布尔型
         * 13、24、改变桌面图标水平和垂直间距，uParam为间距值(像素)，lpvParam为long型
         * 15、设置屏保等待时间，SystemParametersInfo(15, 秒数, P, 1)，lpvParam为long型
         * 20、设置桌面背景墙纸，SystemParametersInfo(20, True, 图片路径, 1)
         * 93、开关鼠标轨迹，SystemParametersInfo(93, 数值, P, 1)，uParam为False则关闭
         * 97、开关Ctrl+Alt+Del窗口，SystemParametersInfo(97, False, A, 0)，uParam为布尔型
         */

        /// <summary>
        /// 系统属性
        /// </summary>
        /// <param name="uAction">指定要设置的参数</param>
        /// <param name="uParam">一个参数，其用法和格式取决于所查询或设置的系统参数。有关系统范围参数的详细信息，请参阅 uiAction 参数。如果未另行指示，则必须为此参数指定零。</param>
        /// <param name="lpvParam">按引用调用的Integer、Long和数据结构</param>
        /// <param name="fuWinIni">这个参数规定了在设置系统参数的时候，是否应更新用户设置参数</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
        );

        public int selectTimes;

        private void ComboBox_WallpaperShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectTimes > 0)
            {
                wallpaperStyle = ComboBox_WallpaperShape.SelectedIndex;
                Registry.SetValue(selfkeypath, "WallPaperStyle", wallpaperStyle);
            }
            selectTimes += 1;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Registry.GetValue(selfkeypath, "WallPaperStyle", null) == null)
            {
                Registry.SetValue(selfkeypath, "WallPaperStyle", 0);
            }
            if (Registry.GetValue(selfkeypath, "WallPaperPath", null) == null)
            {
                Registry.SetValue(selfkeypath, "WallPaperPath", "");
            }
            // 获取
            ComboBox_WallpaperShape.SelectedIndex = (int)Registry.GetValue(selfkeypath, "WallPaperStyle", false);
        }

        private async void Btn_BingApp_Click(object sender, RoutedEventArgs e)
        {
            string file = await Download();
            Thread thread = new(() => SystemParametersInfo(20, 0, file, wallpaperStyle));
            thread.Start();
        }

        private Task<string> Download()
        {
            return Task.Run(() =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(bingWallpaperApi);
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                JObject jObject = JObject.Parse(retString);
                string downloadUrl = String.Format("https://cn.bing.com/{0}", jObject["images"][0]["url"].ToString()) ;
                string hsh = jObject["images"][0]["hsh"].ToString();

                HttpWebRequest requestD = WebRequest.Create(downloadUrl) as HttpWebRequest;
                HttpWebResponse responseD = requestD.GetResponse() as HttpWebResponse;
                Stream responseStream = responseD.GetResponseStream();
                // 防止文件夹不存在报错
                if (!Directory.Exists(wallpaperpath))
                {
                    Directory.CreateDirectory(wallpaperpath);
                }
                Stream stream = new FileStream(wallpaperpath + hsh + ".png", FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                return wallpaperpath + hsh + ".png";
            });
        }
    }
}