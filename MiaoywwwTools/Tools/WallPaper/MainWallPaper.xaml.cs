using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new(() =>
            {
                string[][] keylist = new string[12][];
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
                foreach (string[] item in keylist)
                {
                    object? keyvalue = Registry.GetValue(keypath, item[0], null);
                    if (keyvalue is null)
                    {
                        Registry.SetValue(keypath, item[0], item[1]);
                    }
                }
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    WordSettings_tboxWordContent.Text = Settings.WordContent;
                    WordSettings_tboxDate1.Text = Settings.WordDate1;
                    WordSettings_tboxDate2.Text = Settings.WordDate2;
                    WordSettings_tboxWordColor.Text = Settings.WordColor.ToString();
                    
                    WordSettings_tboxFontSize.Text = Settings.FontSize.ToString();
                    WordSettings_cboxUseWord.IsChecked = Settings.UseWord;

                    VideoSettings_tboxVideoFileName.Text = Registry.GetValue(keypath, "VideoSettings_VideoName", "None").ToString();
                    VideoSettings_labVideoVolume.Content = Settings.VideoVolume.ToString();
                    VideoSettings_cboxVideoLoop.IsChecked = Settings.VideoLoop;
                    VideoSettings_cboxUseVideo.IsChecked = Settings.UseVideo;
                    cboxStartOn.IsChecked = bool.Parse(Registry.GetValue(keypath, "StartOnBoot", "False").ToString());
                    try
                    {
                        Source = new Uri(Registry.GetValue(keypath, "VideoSettings_VideoFilePath", null).ToString());
                    }
                    catch (UriFormatException)
                    {
                        Source = null;
                    }
                    VideoSettings_sliderVideoVolume.Value = double.Parse(Registry.GetValue(keypath, "VideoSettings_VideoVolume", "100.0").ToString());
                    Settings.VideoUri = Source;
                    if (GlobalV.Started)
                    {
                        btnStart.Content = "重设";
                    }
                    else
                    {
                        btnStart.Content = "开始";
                    }
                    /*
                    WordSettings_tboxWordContent.Text = Registry.GetValue(keypath, "WordSettings_WordContent", "").ToString();
                    WordSettings_tboxDate1.Text = Registry.GetValue(keypath, "WordSettings_Date1", null).ToString();
                    WordSettings_tboxDate2.Text = Registry.GetValue(keypath, "WordSettings_Date2", null).ToString();
                    WordSettings_tboxWordColor.Text = Registry.GetValue(keypath, "WordSettings_WordColor", null).ToString();
                    WordSettings_tboxFontSize.Text = Registry.GetValue(keypath, "WordSettings_FontSize", null).ToString();
                    WordSettings_cboxUseWord.IsChecked = bool.Parse(Registry.GetValue(keypath, "WordSettings_UseWord", null).ToString());

                    VideoSettings_tboxVideoFileName.Text = Registry.GetValue(keypath, "VideoSettings_VideoName", "None").ToString();
                    VideoSettings_labVideoVolume.Content = VideoSettings_sliderVideoVolume.Value.ToString();
                    VideoSettings_cboxVideoLoop.IsChecked = bool.Parse(Registry.GetValue(keypath, "VideoSettings_VideoLoop", "False").ToString());
                    VideoSettings_cboxUseVideo.IsChecked = bool.Parse(Registry.GetValue(keypath, "VideoSettings_UseVideo", "False").ToString());
                    cboxStartOn.IsChecked = bool.Parse(Registry.GetValue(keypath, "StartOnBoot", "False").ToString());
                    try
                    {
                        Source = new Uri(Registry.GetValue(keypath, "VideoSettings_VideoFilePath", null).ToString());
                    }
                    catch (UriFormatException)
                    {
                        Source = null;
                    }
                    VideoSettings_sliderVideoVolume.Value = double.Parse(Registry.GetValue(keypath, "VideoSettings_VideoVolume", "100.0").ToString());
                    Settings.VideoUri = Source;
                    if (GlobalV.Started)
                    {
                        btnStart.Content = "重设";
                    }
                    else
                    {
                        btnStart.Content = "开始";
                    }
                    WinMain.winMain.WlpChangeSettings();
                    */
                }));
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            WinMain.winMain.WlpChangeSettings();
            bool isallow = true;
            if ((bool)VideoSettings_cboxUseVideo.IsChecked)
            {
                if (Source is null)
                {
                    MessageBox.ShowDialog("请选择一个视频");
                    isallow = false;
                }
            }

            if (isallow)
            {
                if (!GlobalV.Started)
                {
                    if (WinMain.winMain.WlpChangeSettings()
)
                    {
                        WinMain.winMain.backGround.medMain.Position = TimeSpan.Zero;
                        WinMain.winMain.backGround.ChangeVideo(true);
                        WinMain.winMain.backGround.ChangeWord();
                        WinMain.winMain.backGround.Show();
                        GlobalV.Started = true;
                        btnStart.Content = "重设";
                    }
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
            Registry.SetValue(keypath, "WordSettings_FontSize", WordSettings_tboxFontSize.Text);
            Registry.SetValue(keypath, "WordSettings_UseWord", WordSettings_cboxUseWord.IsChecked.ToString());

            Registry.SetValue(keypath, "VideoSettings_VideoName", VideoSettings_tboxVideoFileName.Text.ToString());
            Registry.SetValue(keypath, "VideoSettings_VideoVolume", VideoSettings_sliderVideoVolume.Value.ToString());
            Registry.SetValue(keypath, "VideoSettings_VideoLoop", VideoSettings_cboxVideoLoop.IsChecked.ToString());
            Registry.SetValue(keypath, "VideoSettings_UseVideo", VideoSettings_cboxUseVideo.IsChecked.ToString());
            Registry.SetValue(keypath, "StartOnBoot", cboxStartOn.IsChecked.ToString());
            if (WinMain.winMain.WlpChangeSettings())
            {
                MessageBox.ShowDialog("应用成功！");
            }
        }

        private void VideoSettings_cboxVideoLoop_Click(object sender, RoutedEventArgs e)
        {
            Settings.VideoLoop = VideoSettings_cboxVideoLoop.IsChecked;
        }

        private void VideoSettings_sliderVideoVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoSettings_labVideoVolume.Content = VideoSettings_sliderVideoVolume.Value.ToString();
            Settings.VideoVolume = VideoSettings_sliderVideoVolume.Value;
            if (WinMain.winMain.backGround != null)
            {
                WinMain.winMain.backGround.ChangeVideo(false);
            }
        }

        private void WordSettings_tboxWordColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();
            try
            {
                WordSettings_boerColorCard.Background = (Brush)brushConverter.ConvertFromString(WordSettings_tboxWordColor.Text);
                Settings.WordColor = WordSettings_boerColorCard.Background;
                if (WinMain.winMain.backGround != null)
                {
                    WinMain.winMain.backGround.ChangeWord();
                }
                WordSettings_boerColorCard.BorderBrush = (Brush)brushConverter.ConvertFromString("#FFE0E0E0");
                Registry.SetValue(keypath, "WordSettings_WordColor", WordSettings_tboxWordColor.Text);
            }
            catch (Exception)
            {
                WordSettings_boerColorCard.BorderBrush = (Brush)brushConverter.ConvertFromString("#FFFF0000");
            }
        }

        private void WordSettings_tboxWordContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.WordContent = WordSettings_tboxWordContent.Text;
            if (WinMain.winMain.backGround != null)
            {
                WinMain.winMain.backGround.ChangeWord();
            }
        }

        private void WordSettings_tboxFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (double.Parse(WordSettings_tboxFontSize.Text) < 0)
                {
                    MessageBox.ShowDialog("请输入一个正整数");
                    WordSettings_tboxFontSize.Text = "100";
                }
                else
                {
                    Settings.FontSize = double.Parse(WordSettings_tboxFontSize.Text);
                    Registry.SetValue(keypath, "WordSettings_FontSize", WordSettings_tboxFontSize.Text);
                    if (WinMain.winMain.backGround != null)
                    {
                        WinMain.winMain.backGround.labContent.FontSize = (double)Settings.FontSize;
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.ShowDialog("请输入一个正确的数字");
                WordSettings_tboxFontSize.Text = "100";
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