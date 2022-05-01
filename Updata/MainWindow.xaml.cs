using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace updata
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

        public string keypath = @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\Updata";
        public string temppath = @$"{Path.GetTempPath()}MiaoywwwTools\updata\";
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

            Registry.SetValue(keypath, "updataFileUUID", uuid);
            //下载远程文件
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    string downloadurl = $@"{temppath}{uuid}.zip";
                    Stream netStream = response.GetResponseStream();
                    Stream fileStream = new FileStream(downloadurl, FileMode.Create);
                    byte[] read = new byte[1024];
                    long progressBarValue = 0;
                    int realReadLen = netStream.Read(read, 0, read.Length);
                    while (realReadLen > 0)
                    {
                        fileStream.Write(read, 0, realReadLen);
                        progressBarValue += realReadLen;
                        this.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            this.pgbar.Value = progressBarValue;
                        }));

                        realReadLen = netStream.Read(read, 0, read.Length);
                    }
                    netStream.Close();
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"下载失败，因为\n{ex}");
                }
                

                string[] filelist =
                {
                    "HandyControl.dll",
                    "updata.deps.json",
                    "updata.dll",
                    "updata.exe",
                    "updata.pdb",
                    "updata.runtimeconfig.json"
                };
                if (!Directory.Exists(temppath))
                {
                    Directory.CreateDirectory(temppath);
                }
                foreach (string filename in filelist)
                {
                    File.Copy(Environment.CurrentDirectory + @"\" + filename, temppath + filename, true);
                }
                ProcessStartInfo process = new ProcessStartInfo();
                process.FileName = temppath + "updata.exe";
                process.Arguments = "true";
                Process.Start(process);
                Registry.SetValue(keypath, "unpackPath", Environment.CurrentDirectory + @"\");
                Environment.Exit(0);
            });
        }

        private void UnpackZip()
        {
            string zippath = @$"{temppath}{Registry.GetValue(keypath, "updataFileUUID", null)}.zip";
            string unpackpath = $"{Registry.GetValue(keypath, "unpackPath", null)}";
            ZipFile.ExtractToDirectory(zippath, unpackpath, true);
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = unpackpath + @"\MiaoywwwTools.exe";
            process.Arguments = "Updataed";
            Process.Start(process);
            File.Delete(zippath);
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            }
        }
    }
}