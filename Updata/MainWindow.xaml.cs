using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Updata
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

            //下载远程文件
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Stream netStream = response.GetResponseStream();
                Stream fileStream = new FileStream("./downloadtemp.zip", FileMode.Create);
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
                GlobalV.downloadfinsh = true;
            }, null);
            
        }

        private void UnpackZip()
        {
            while (true)
            {
                if (GlobalV.downloadfinsh)
                {
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        Label_Content.Content = "解压中";
                    }));
                    ZipFile.ExtractToDirectory("./downloadtemp.zip", "./", true);
                    Process.Start(Environment.CurrentDirectory + @"\MiaoywwwTools.exe");
                    Environment.Exit(0);
                }
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalV.downloadurl is not "")
            {
                Label_Content.Content = "下载中";
                Task download = new Task(Download);
                download.Start();
                Task unpack = new Task(UnpackZip);
                unpack.Start();
            }
        }
    }
}