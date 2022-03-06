using System;
using System.Windows;
using System.Windows.Input;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinMessage.xaml 的交互逻辑
    /// </summary>
    public partial class WinMessage : Window
    {
        public static WinMessage? winMessage;
        public string MessageTitle;
        public string MessageBody;
        public string MessageAction;

        public WinMessage()
        {
            InitializeComponent();
            winMessage = this;
        }

        public string action;

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
                    Btn_No.Visibility = Visibility.Visible;
                    return;

                case "no":
                    Btn_Yes.Visibility = Visibility.Visible;
                    return;
            }
        }

        private void WindowsAction(string context)
        {
            switch (context)
            {
                case "close":
                    this.Close();
                    break;

                case "closeall":
                    WinMain.winMain.Close();
                    break;

                case "restart":
                    Application.Current.Shutdown();
                    break;
            }
        }

        private void Btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            WindowsAction(action);
        }

        private void Btn_No_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (action == "restart")
            {
                System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);//重启软件
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}