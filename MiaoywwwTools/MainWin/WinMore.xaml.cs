using MiaoywwwTools.Tools.RandomDraw;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace MiaoywwwTools
{
    /// <summary>
    /// WinMore.xaml 的交互逻辑
    /// </summary>
    public partial class WinMore : Page
    {
        public WinMore()
        {
            InitializeComponent();
        }

        public string PageName;

        public void ChangePage(string pagename)
        {
            if (pagename != PageName)
            {
                Type pageType = Type.GetType(pagename);
                if (pageType != null)
                {
                    this.NestPage.Content = new Frame()
                    {
                        Content = Activator.CreateInstance(pageType)
                    };
                }
                PageName = pagename;
            }
        }

        // 点名器
        private void Btn_RandomDraw_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.Tools.RandomDraw.MainRandomDraw");
        }

        private void Btn_WallPaper_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangePage("MiaoywwwTools.Tools.WallPaper.MainWallPaper");
        }
    }
}