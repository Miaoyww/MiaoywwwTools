using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RandomDrawLib;
using System.Drawing;

namespace MiaoywwwTools.Tools.RandomDraw
{
    /// <summary>
    /// ShowResult.xaml 的交互逻辑
    /// </summary>
    public partial class ShowResult : Window
    {
        public ShowResult showResult;
        public ShowResult()
        {
            InitializeComponent();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
                raDraw.Read();
                string[] result = raDraw.GetRandomResult();
                Label_Name.Content = result[0];
                Label_Grade.Content = result[1];
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;//还原窗口（非最小化和最大化）
            this.WindowStyle = System.Windows.WindowStyle.None; //仅工作区可见，不显示标题栏和边框
            this.ResizeMode = System.Windows.ResizeMode.NoResize;//不显示最大化和最小化按钮
            // this.Topmost = true;    //窗口在最前
            this.Activate();
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            WinMain.winMain.Login = false;
            this.Close();
        }
    }
}
