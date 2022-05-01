using HandyControl.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Windows.Controls;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinSettings.xaml 的交互逻辑
    /// </summary>
    public partial class WinSettings : Page
    {
        public WinSettings()
        {
            InitializeComponent();
        }

        public string getAppVersion_time;
        public string getAppVersion_ver;
        public bool getdone = false;
        public JObject? jsonContent;
        public int SourceSelected;
        private void Btn_CheckUpdata_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Btn_CheckUpdata.IsEnabled = false;
            ldCircle_CheckUpdata.IsEnabled = true;
            ldCircle_CheckUpdata.Visibility = System.Windows.Visibility.Visible;
            string api_version;
            string api_download;
            SourceSelected = Cbox_Source.SelectedIndex;
            if (SourceSelected == 0)
            {
                api_version = "https://api.github.com/repos/Miaoywww/MiaoywwwTools/contents/MiaoywwwTools/version.json";
                api_download = "https://api.github.com/repos/Miaoywww/MiaoywwwTools/releases";
            }
            else
            {
                api_version = "https://gitee.com/miaoywww/MiaoywwwTools/raw/main/MiaoywwwTools/version.json";
                api_download = "https://gitee.com/api/v5/repos/Miaoywww/MiaoywwwTools/releases/latest";
            }
            Thread check = new Thread(() =>
            {
                HttpClient httpClientV = new HttpClient();
                httpClientV.DefaultRequestHeaders.Add("Accept", "application/vnd.github.VERSION.raw");
                httpClientV.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                var downloadV = httpClientV.GetStringAsync(api_version).Result;

                JObject jsonContentV = (JObject)JsonConvert.DeserializeObject(downloadV);
                int? time = int.Parse(jsonContentV["MiaoywwwTools"]["time"].ToString());
                string? ver = jsonContentV["MiaoywwwTools"]["version"].ToString();

                if (int.Parse(GlobalV.AppVersion_time) < time)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        var messageBox = MessageBox.ShowDialog($"检测到新版本{ver},是否更新?");
                        if (messageBox.IsYes)
                        {
                            HttpClient httpClientD = new HttpClient();
                            httpClientD.DefaultRequestHeaders.Add("Accept", "application/vnd.github.VERSION.raw");
                            httpClientD.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                            var download = httpClientD.GetStringAsync(api_download).Result;
                            JObject jsonContentD;
                            if (SourceSelected == 0)
                            {
                                JArray jsonArrayD = JArray.Parse(download);
                                jsonContentD = JObject.Parse(jsonArrayD[0].ToString());
                            }
                            else
                            {
                                jsonContentD = (JObject)JsonConvert.DeserializeObject(download);
                            }
                            string downloadurl = jsonContentD["assets"][0]["browser_download_url"].ToString();
                            ProcessStartInfo process = new ProcessStartInfo();
                            process.FileName = Environment.CurrentDirectory + @"\updata.exe";
                            process.Arguments = downloadurl;
                            Process.Start(process);
                            Environment.Exit(0);
                        }
                    });
                }
                else
                {
                    if (int.Parse(GlobalV.AppVersion_time) == time || int.Parse(GlobalV.AppVersion_time) > time)
                    {
                        Dispatcher.BeginInvoke(() => { MessageBox.ShowDialog("当前已经是最新版本了"); });
                    }
                }
                this.Dispatcher.BeginInvoke(() =>
                {
                    Btn_CheckUpdata.IsEnabled = true;
                    ldCircle_CheckUpdata.IsEnabled = false;
                    ldCircle_CheckUpdata.Visibility = System.Windows.Visibility.Hidden;
                });
            });
            check.IsBackground = true;
            check.Start();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Label_Version.Content = $"By Miaomiaoywww 2022 -Version {GlobalV.AppVersion_ver}";
            string rgValue = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "Hitokoto", "random").ToString();
            Cbox_Hitokoto.SelectedIndex = int.Parse(GlobalV.Hitokoto_To_Index[rgValue].ToString());
        }

        private void Btn_GoToHitokoto_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://hitokoto.cn/");
        }

        private void Cbox_Hitokoto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "Hitokoto", GlobalV.Index_To_Hitokoto[Cbox_Hitokoto.SelectedIndex.ToString()]);
        }
    }
}