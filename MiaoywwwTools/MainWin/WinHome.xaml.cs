using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinHome.xaml 的交互逻辑
    /// </summary>
    public partial class WinHome : Page
    {
        public WinHome winHome;

        public WinHome()
        {
            InitializeComponent();
            if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "HitokotoType", null) is null)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "HitokotoType", "random");
            }
            timer_onesay.Tick += new EventHandler(TimerOneSay_Tick);
            timer_onesay.Interval = TimeSpan.FromSeconds(10); //设置刷新的间隔时间
            timer_onesay.Start();
            string HitokotoLastContent = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                "HitokotoLastContent", "MiaoywwwTools").ToString();
            string HitokotoLastFrom = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                "HitokotoLastFrom", "Miaomiaoywww").ToString();
            string HitokotoLastFromWho = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                "HitokotoLastFromWho", "").ToString();
            Label_HitokotoContent.Content = HitokotoLastContent;
            Label_HitokotoFrom.Content = "-" + HitokotoLastFrom;
            winHome = this;
        }

        public int lastTitleClickTimes;
        public int nowTitleClickTimes;
        public bool animationCompleted = true;

        public JObject HttpGetJson(string url)
        {
            HttpClient httpClientV = new HttpClient();
            httpClientV.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            var downloadV = httpClientV.GetStringAsync(url).Result;

            JObject jsonContentV = (JObject)JsonConvert.DeserializeObject(downloadV);
            return jsonContentV;
        }

        // Miaomiaoywww的头像下载 **需要更改
        public static void DownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            // 防止文件夹不存在报错
            if (!Directory.Exists(System.Environment.CurrentDirectory + @"\Resources\Images\"))
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\Resources\Images\");
            }
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
        }

        // 彩蛋的文字变更
        public void ChangeEmotion(string emotion)
        {
            // 先让动画播放完毕，防止动画出现问题
            if (animationCompleted is true)
            {
                var story = (Storyboard)this.Resources["ShowLabel"];
                if (story != null)
                {
                    story.Remove();
                    Label_Emotion.Content = emotion;
                    story.Begin(Label_Emotion, true);
                    animationCompleted = false;
                    lastTitleClickTimes = nowTitleClickTimes;
                }
            }
            else
            {
                // 防止动画播放时，继续点击的次数计入
                nowTitleClickTimes = lastTitleClickTimes;
            }
        }

        // 彩蛋的入口
        private void Label_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            nowTitleClickTimes++;
            switch (nowTitleClickTimes)
            {
                case 3:
                    ChangeEmotion("ヾ(•ω•`)o");
                    return;

                case 6:
                    ChangeEmotion("(*/ω＼*)");
                    return;

                case 10:
                    ChangeEmotion("✪ ω ✪");
                    return;

                case 14:
                    ChangeEmotion(".·´¯`(>▂<)´¯`·. ");
                    return;

                case 16:
                    ChangeEmotion("(っ °Д °))っ");
                    return;

                case 18:
                    ChangeEmotion("（＞人＜；）");
                    return;

                case 20:
                    ChangeEmotion("(´。＿。｀)");
                    return;
            }
        }

        // 这是彩蛋内容显现动画完成事件
        private void ShowLabel_Completed(object sender, EventArgs e)
        {
            var story = (Storyboard)this.Resources["HideLabel"];
            if (story != null)
            {
                story.Begin(Label_Emotion);
            }
        }

        // 这是彩蛋内容隐藏动画完成事件
        private void HideLabel_Completed(object sender, EventArgs e)
        {
            if (nowTitleClickTimes >= 20)
            {
                WinMain.winMain.ChangePage("MiaoywwwTools.EasterEgg");
            }
            animationCompleted = true;
        }

        // 各种头像的文件位置

        /// <summary>
        /// ToolsRr的注册表目录
        /// </summary>
        public string keypath = "HKEY_CURRENT_USER\\SOFTWARE\\Miaoywww\\MiaoywwwTools\\";

        private DispatcherTimer timer_onesay = new DispatcherTimer();

        public JObject HitokotoJson;
        public string HitokotoContent;
        public string HitokotoFrom;
        public string HitokotoFromWho;

        private void TimerOneSay_Tick(object sender, EventArgs e)
        {
            Thread changeTime = new(() =>
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
                    HitokotoContent = HitokotoJson["hitokoto"].ToString();
                    HitokotoFrom = HitokotoJson["from"].ToString();
                    HitokotoFromWho = HitokotoJson["from_who"].ToString();
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                        "HitokotoLastContent", HitokotoContent);
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                        "HitokotoLastFrom", HitokotoFrom);
                    Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\",
                        "HitokotoLastFromWho", HitokotoFromWho);
                }
                catch (Exception)
                {
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    Label_HitokotoContent.Content = $"{HitokotoContent}";
                    if (HitokotoFromWho is not "")
                    {
                        Label_HitokotoFrom.Content = $"-{HitokotoFrom} / {HitokotoFromWho}";
                    }
                    else
                    {
                        Label_HitokotoFrom.Content = $"-{HitokotoFrom}";
                    }
                }));
                Thread.Sleep(0);
            });
            changeTime.IsBackground = true;
            changeTime.Start();
        }

        // 页面加载时
        private void MainGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}