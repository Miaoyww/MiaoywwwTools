using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
        public static WinHome winHome;
        private DispatcherTimer timer_onesay = new DispatcherTimer();

        public WinHome()
        {
            InitializeComponent();
            winHome = this;
            if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "HitokotoType", null) is null)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "HitokotoType", "random");
            }
            timer_onesay.Tick += new EventHandler(TimerOneSay_Tick);
            timer_onesay.Interval = TimeSpan.FromSeconds(3); //设置刷新的间隔时间
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

        public void TimerOneSay_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (GlobalV.HitokotoContent != null)
                {
                    Label_HitokotoContent.Content = $"{GlobalV.HitokotoContent}";
                    if (GlobalV.HitokotoFromWho is not "")
                    {
                        Label_HitokotoFrom.Content = $"-{GlobalV.HitokotoFrom} / {GlobalV.HitokotoFromWho}";
                    }
                    else
                    {
                        Label_HitokotoFrom.Content = $"-{GlobalV.HitokotoFrom}";
                    }
                }
            }));
        }

        // 页面加载时
        private void MainGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}