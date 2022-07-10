using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows;

namespace update
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string keypath = @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\update";
        public string temppath = @$"{Path.GetTempPath()}MiaoywwwTools\update\";
        public string uuid = Guid.NewGuid().ToString("N");

        private void Download()
        {
            WebResponse? response = null;
            //获取远程文件
            WebRequest request = WebRequest.Create(GlobalV.downloadurl);
            response = request.GetResponse();
            if (response == null) return;
            //读远程文件的大小
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                pgbar.Maximum = response.ContentLength;
            }));

            Registry.SetValue(keypath, "updateFileUUID", uuid);
            //下载远程文件
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    string downloadurl = $@"{temppath}{uuid}.zip";
                    if (!Directory.Exists(temppath))
                    {
                        Directory.CreateDirectory(temppath);
                    }
                    Stream netStream = response.GetResponseStream();
                    Stream fileStream = new FileStream(downloadurl, FileMode.Create);
                    byte[] read = new byte[1024];
                    long progressBarValue = 0;
                    int realReadLen = netStream.Read(read, 0, read.Length);
                    string currentSize;
                    string targetSize = (((double)response.ContentLength) / 1024 / 1024).ToString();
                    targetSize = $"{targetSize.Split(".")[0]}.{targetSize.Split(".")[1].Substring(0, 2)}";
                    while (realReadLen > 0)
                    {
                        fileStream.Write(read, 0, realReadLen);
                        progressBarValue += realReadLen;
                        currentSize = (((double)progressBarValue) / 1024 / 1024).ToString();
                        currentSize = $"{currentSize.Split(".")[0]}.{currentSize.Split(".")[1].Substring(0,2)}";
                        this.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            this.pgbar.Value = progressBarValue;
                            Label_TargetSize.Content = $"{currentSize} / {targetSize} MB";
                        }));
                        realReadLen = netStream.Read(read, 0, read.Length);
                    }
                    netStream.Close();
                    fileStream.Close();

                    JObject updateInfoJs = new JObject();
                    updateInfoJs.Add(new JProperty("update_finsh", "true"));
                    updateInfoJs.Add(new JProperty("download_url", GlobalV.downloadurl));
                    updateInfoJs.Add(new JProperty("target_version", GlobalV.targetversion));
                    updateInfoJs.Add(new JProperty("current_version", GlobalV.currentversion));
                    File.WriteAllText("./update_info.json", updateInfoJs.ToString(), System.Text.Encoding.UTF8);//将内容写进jon文件中
                    string[] filelist =
                     {
                    "Newtonsoft.Json.dll",
                    "HandyControl.dll",
                    "update.deps.json",
                    "update.dll",
                    "update.exe",
                    "update.pdb",
                    "update.runtimeconfig.json",
                    "update_info.json",
                    @"resources\Data\stdata.json"
                     };
                    if (!Directory.Exists(@$"{temppath}\resources\Data"))
                    {
                        Directory.CreateDirectory(@$"{temppath}\resources\Data");
                    }
                    foreach (string filename in filelist)
                    {
                        File.Copy(Environment.CurrentDirectory + @"\" + filename, temppath + filename, true);
                    }
                    ProcessStartInfo process = new ProcessStartInfo();
                    process.FileName = temppath + "update.exe";
                    Process.Start(process);
                    Registry.SetValue(keypath, "unpackPath", Environment.CurrentDirectory + @"\");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"更新失败，因为\n{ex}");
                    ProcessStartInfo process = new ProcessStartInfo();
                    string unpackpath = $"{Registry.GetValue(keypath, "unpackPath", null)}";
                    process.FileName = unpackpath + @"\MiaoywwwTools.exe";
                    Process.Start(process);
                }
            });
        }

        private void UnpackZip()
        {
            try
            {
                string zippath = @$"{temppath}{Registry.GetValue(keypath, "updateFileUUID", null)}.zip";
                string unpackpath = $"{Registry.GetValue(keypath, "unpackPath", null)}";
                ZipFile.ExtractToDirectory(zippath, unpackpath, true);
                ProcessStartInfo process = new ProcessStartInfo();
                process.FileName = unpackpath + @"\MiaoywwwTools.exe";
                process.Arguments = "updateed";
                Process.Start(process);
                // File.Copy(@".\resources\Data\stdata.json", @$"{unpackpath}\resources\Data\stdata.json", true);
                File.Delete(zippath);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新失败，因为\n{ex}");
                ProcessStartInfo process = new ProcessStartInfo();
                string unpackpath = $"{Registry.GetValue(keypath, "unpackPath", null)}";
                process.FileName = unpackpath + @"\MiaoywwwTools.exe";
                Process.Start(process);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   /*
            if (GlobalV.downloadurl is not "")
            {
                Label_Content.Content = "下载中";
                Thread download = new Thread(Download);
                download.IsBackground = true;
                download.Start();
            }
            if (GlobalV.unpack is not false)
            {
                Label_Content.Content = "解压中";
                pgbar.Value = 100;
                Thread unpack = new Thread(UnpackZip);
                unpack.IsBackground = true;
                unpack.Start();
            }*/
            if (!File.Exists("./update_info.json"))
            {
                Environment.Exit(1);
            }
            else
            {
                StreamReader reader = File.OpenText(@"./update_info.json");
                JObject jsonContent = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                GlobalV.downloadfinsh = bool.Parse(jsonContent["update_finsh"].ToString());
                GlobalV.targetversion = jsonContent["target_version"].ToString();
                GlobalV.currentversion = jsonContent["current_version"].ToString();
                if (!GlobalV.downloadfinsh)
                {
                    GlobalV.downloadurl = jsonContent["download_url"].ToString();
                    Label_Content.Content = "下载中";
                    Label_TargetVersion.Content = $"目标版本: {GlobalV.targetversion}";
                    Label_CurrentVersion.Content = $"当前版本: {GlobalV.currentversion}";
                    Thread download = new Thread(Download);
                    download.IsBackground = true;
                    download.Start();
                }
                else
                {
                    Label_Content.Content = "解压中";
                    pgbar.Value = 100;
                    Label_TargetVersion.Content = $"目标版本: {GlobalV.targetversion}";
                    Label_CurrentVersion.Content = $"当前版本: {GlobalV.currentversion}";
                    Label_TargetSize.Content = "";
                    Thread unpack = new Thread(UnpackZip);
                    unpack.IsBackground = true;
                    unpack.Start();
                }

            }
        }
    }
}