using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace MiaoywwwTools
{
    /// <summary>
    /// WinMain.xaml 的交互逻辑
    /// </summary>

#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
    public partial class WinMain : Window
    {
        public static WinMain? winMain;
        public WinMain()
        {
            InitializeComponent();
            winMain = this;

            /*
            RaDraw a = new RaDraw();
            #pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
            string[] result = a.GetRandomResult();
            #pragma warning disable CS8602 // 解引用可能出现空引用。
            MessageBox.Show(string.Format("{0}\n{1}", result[0], result[1]));s
            */
        }
        /// <summary>
        /// 切换窗口
        /// </summary>
        /// <param name="pagename"> 页面名称 </param>
        public void ChangePage(string pagename)
        {
            Type pageType = Type.GetType(pagename);
            if (pageType != null)
            {
                this.NestPage.Content = new Frame()
                {
                    Content = Activator.CreateInstance(pageType)
                };
            }
        }

        // DragMove 窗口移动
        private void Border_Top_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        // 导航栏
        private void Btns_Home_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinHome");
        }
        private void Btns_More_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinMore");
        }

        // 关闭按钮，转到动画
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Close(); };
                story.Begin(this);
            }
        }

        // 开机主页
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.WinHome");
        }
    }
}
