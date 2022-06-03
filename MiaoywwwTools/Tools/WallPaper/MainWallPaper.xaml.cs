using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MiaoywwwTools;
namespace MiaoywwwTools.Tools.WallPaper
{
    /// <summary>
    /// WallPaper.xaml 的交互逻辑
    /// </summary>
    public partial class MainWallPaper : Page
    {
        public string keypath = @"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\Tools\WallPaper";
        private Uri? Source;
        

        public MainWallPaper()
        {
            InitializeComponent();
        }

        internal class Settings
        {
            public static bool? UseVideo;
            public static bool? UseWord;

            public static bool? VideoLoop;
            public static Uri? VideoUri;
            public static double? VideoVolume;

            public static Brush? WordColor;
            public static string? WordContent;
            public static string? WordDate1;
            public static string? WordDate2;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string[][] keylist = new string[11][];
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
            foreach (string[] item in keylist)
            {
                object? keyvalue = Registry.GetValue(keypath, item[0], null);
                if (keyvalue is null)
                {
                    Registry.SetValue(keypath, item[0], item[1]);
                }
            }
            WordSettings_tboxWordContent.Text = Registry.GetValue(keypath, "WordSettings_WordContent", "").ToString();
            WordSettings_tboxDate1.Text = Registry.GetValue(keypath, "WordSettings_Date1", null).ToString();
            WordSettings_tboxDate2.Text = Registry.GetValue(keypath, "WordSettings_Date2", null).ToString();
            WordSettings_tboxWordColor.Text = Registry.GetValue(keypath, "WordSettings_WordColor", null).ToString();
            WordSettings_cboxUseWord.IsChecked = bool.Parse(Registry.GetValue(keypath, "WordSettings_UseWord", null).ToString());

            VideoSettings_tboxVideoFileName.Text = Registry.GetValue(keypath, "VideoSettings_VideoName", "None").ToString();
            VideoSettings_labVideoVolume.Content = VideoSettings_sliderVideoVolume.Value.ToString();
            VideoSettings_cboxVideoLoop.IsChecked = bool.Parse(Registry.GetValue(keypath, "VideoSettings_VideoLoop", "False").ToString());
            VideoSettings_cboxUseVideo.IsChecked = bool.Parse(Registry.GetValue(keypath, "VideoSettings_UseVideo", "False").ToString());
            cboxStartOn.IsChecked = bool.Parse(Registry.GetValue(keypath, "StartOnBoot", "False").ToString());
            Source = new Uri(Registry.GetValue(keypath, "VideoSettings_VideoFilePath", null).ToString());
            Settings.VideoUri = Source;
            if (GlobalV.Started)
            {
                btnStart.Content = "重设";
            }
            else
            {
                btnStart.Content = "开始";
            }
            ChangeSettings();
            VideoSettings_sliderVideoVolume.Value = double.Parse(Registry.GetValue(keypath, "VideoSettings_VideoVolume", "100.0").ToString());
            WinMain.winMain.backGround = new();
            WinMain.winMain.backGround.medMain.Stop();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ChangeSettings();
            bool isallow = false;
            if ((bool)VideoSettings_cboxUseVideo.IsChecked)
            {
                if (Source is null)
                {
                    MessageBox.ShowDialog("请选择一个视频");
                }
                else
                {
                    isallow = true;
                }
            }


            if (isallow)
            {
                if (!GlobalV.Started)
                {
                    WinMain.winMain.backGround.ChangeVideo();
                    WinMain.winMain.backGround.ChangeWord();
                    WinMain.winMain.backGround.Show();
                    GlobalV.Started = true;
                    btnStart.Content = "重设";
                }
                else
                {
                    WinMain.winMain.backGround.Hide();
                    WinMain.winMain.backGround.medMain.Stop();
                    WinMain.winMain.backGround.medMain.Source = null;
                    GlobalV.Started = false;
                    btnStart.Content = "开始";
                }
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            Registry.SetValue(keypath, "WordSettings_WordContent", WordSettings_tboxWordContent.Text);
            Registry.SetValue(keypath, "WordSettings_Date1", WordSettings_tboxDate1.Text);
            Registry.SetValue(keypath, "WordSettings_Date2", WordSettings_tboxDate2.Text);
            Registry.SetValue(keypath, "WordSettings_WordColor", WordSettings_tboxWordColor.Text);
            Registry.SetValue(keypath, "WordSettings_UseWord", WordSettings_cboxUseWord.IsChecked.ToString());

            Registry.SetValue(keypath, "VideoSettings_VideoName", VideoSettings_sliderVideoVolume.Value.ToString());
            Registry.SetValue(keypath, "VideoSettings_VideoVolume", VideoSettings_sliderVideoVolume.Value.ToString());
            Registry.SetValue(keypath, "VideoSettings_VideoLoop", VideoSettings_cboxVideoLoop.IsChecked.ToString());
            Registry.SetValue(keypath, "VideoSettings_UseVideo", VideoSettings_cboxUseVideo.IsChecked.ToString());
            Registry.SetValue(keypath, "StartOnBoot", cboxStartOn.IsChecked.ToString());
            WinMain.winMain.backGround.ChangeVideo();
            WinMain.winMain.backGround.ChangeWord();
            if (ChangeSettings())
            {
                MessageBox.ShowDialog("应用成功！");
            }

        }

        private bool ChangeSettings()
        {
            Settings.UseVideo = VideoSettings_cboxUseVideo.IsChecked;
            Settings.UseWord = WordSettings_cboxUseWord.IsChecked;

            Settings.VideoVolume = VideoSettings_sliderVideoVolume.Value;
            Settings.VideoLoop = VideoSettings_cboxVideoLoop.IsChecked;
            Settings.VideoUri = Source;

            Settings.WordContent = WordSettings_tboxWordContent.Text;
            try
            {
                BrushConverter brushConverter = new BrushConverter();
                Settings.WordColor = (Brush)brushConverter.ConvertFromString(WordSettings_tboxWordColor.Text);
                WordSettings_boerColorCard.Background = Settings.WordColor;
            }
            catch (FormatException)
            {
                MessageBox.ShowDialog("请输入一个正确的ARGB颜色");
                return false;
            }
            try
            {
                if (WordSettings_tboxDate1.Text != "Now")
                {
                    DateTime dt1 = Convert.ToDateTime(WordSettings_tboxDate1.Text);
                }
                if (WordSettings_tboxDate2.Text != "Now")
                {
                    DateTime dt2 = Convert.ToDateTime(WordSettings_tboxDate2.Text);
                }
                Settings.WordDate1 = WordSettings_tboxDate1.Text;
                Settings.WordDate2 = WordSettings_tboxDate2.Text;
            }
            catch (FormatException)
            {
                MessageBox.ShowDialog("请输入一个正确的日期,如2022-6-14");
                return false;
            }
            return true;
        }

        private void VideoSettings_sliderVideoVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoSettings_labVideoVolume.Content = VideoSettings_sliderVideoVolume.Value.ToString();
            Settings.VideoVolume = VideoSettings_sliderVideoVolume.Value;
            if(WinMain.winMain.backGround != null)
            {
                WinMain.winMain.backGround.ChangeVideo();
            }

        }

        private void VideoSettings_btnChooseVideoFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                DefaultExt = ".mp4",
                Filter = "视频文件(.MP4)|*.mp4;"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Registry.SetValue(keypath, "VideoSettings_VideoFilePath", openFileDialog.FileName);
                Registry.SetValue(keypath, "VideoSettings_VideoName", Path.GetFileName(openFileDialog.FileName));
                string[] namesplit = openFileDialog.FileName.Split(".");
                VideoSettings_tboxVideoFileName.Text = Path.GetFileName(openFileDialog.FileName);
                Source = new Uri(openFileDialog.FileName);
            }
        }
    }
}