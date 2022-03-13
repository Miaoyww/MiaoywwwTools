using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinMessage.xaml 的交互逻辑
    /// </summary>
    public partial class WinMessage : Window
    {
        public static WinMessage? winMessage;

        public WinMessage()
        {
            InitializeComponent();
            winMessage = this;
        }

        public string action;

        /// <summary>
        /// 设置信息窗口的各种信息
        /// </summary>
        /// <param name="title"> 信息窗口的标题 </param>
        /// <param name="content"> 信息窗口的内容 </param>
        /// <param name="act"> 信息窗口的行为 </param>
        /// <param name="yesno"> 确认取消按钮的显示与隐藏 </param>
        public void SetMessage(string title, string content, string act, string yesno)
        {
            Label_MessageTitle.Content = title;
            Label_MessageBody.Content = content;
            action = act;
            switch (yesno)
            {
                case "yesno":
                    return;

                case "yes":
                    Btn_No.Visibility = Visibility.Hidden;
                    return;

                case "no":
                    Btn_Yes.Visibility = Visibility.Hidden;
                    return;
            }
        }

        private void WindowsAction(string context)
        {
            switch (context)
            {
                case "close":
                    CloseWindow();
                    break;

                case "closeall":
                    WinMain.winMain.CloseWindow();
                    break;

                case "restart":
                    Application.Current.Shutdown();
                    break;
            }
        }

        private void CloseWindow()
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Close(); };
                story.Begin(this);
            }
            // this.Close();
        }

        private void Btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            WindowsAction(action);
        }

        private void Btn_No_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (action == "restart")
            {
                System.Diagnostics.Process.Start(Environment.ProcessPath);//重启软件
            }
        }

        private void Border_Top_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}