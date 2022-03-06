﻿using System;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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
            winHome = this;
        }

        public int lastTitleClickTimes;
        public int nowTitleClickTimes;
        public bool animationCompleted = true;

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
        public string MiaoywwwfacePath = System.Environment.CurrentDirectory + @"\Resources\Images\MiaoywwwFace.png";
        public string userfacePath = System.Environment.CurrentDirectory + @"\Resources\Images\UserFace.png";
        public string userfacetempPath = System.Environment.CurrentDirectory + @"\Resources\Images\UserFacetemp.png";


        // 头像Image点击事件
        private void Image_Face_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 选择文件
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "选择你的头像";
            dialog.DefaultExt = ".png";
            dialog.Filter = @"图像文件(*.jpg,*.png,*.tif,*.gif)|*jpeg;*.jpg;*.png;*.tif;*.tiff;*.gif
            |JPEG(*.jpeg, *.jpg)|*.jpeg;*.jpg|PNG(*.png)|*.png";
            // 打开选择框选择
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string selectfilePath = dialog.FileName; // 获取选择的文件名
                File.Copy(selectfilePath, userfacetempPath);
                WinMessage winMessage = new WinMessage();
                WinMessage.winMessage.SetMessageTitle("信息");
                WinMessage.winMessage.SetMessageBody("本操作需要重启程序,是否继续?");
                WinMessage.winMessage.SetMessageAction("restart");
                winMessage.ShowDialog();
            }
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

        // 页面加载时
        private void MainGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 判断是否有用户temp头像
            if (File.Exists(userfacetempPath))
            {
                File.Delete(userfacePath);
                File.Move(userfacetempPath, userfacePath);
            }
            // 判断是否有用户头像
            if (File.Exists(userfacePath))
            {
                Image_Face.Source = new BitmapImage(new Uri(userfacePath));
                Label_Name.Content = "这是你哦！";
            }
            else // 无则使用Miaomiaoywww的头像
            {
                // 判断是否有
                if (File.Exists(MiaoywwwfacePath))
                {
                    Image_Face.Source = new BitmapImage(new Uri(MiaoywwwfacePath));
                    Label_Name.Content = "MiaoMiaoywww";
                }
                else // 无则下载
                {
                    DownloadFile("http://q1.qlogo.cn/g?b=qq&nk=375629202&s=640", MiaoywwwfacePath);
                    Image_Face.Source = new BitmapImage(new Uri(MiaoywwwfacePath));
                    Label_Name.Content = "Miaomiaoywww";
                }
            }

        }
        // to Github
        private void BtnGoTo_Github_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Miaoywww");
        }
        // to Bilibili
        private void BtnGoTo_Bilibili_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://space.bilibili.com/435970102");
        }
    }
}
