using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiaoywwwTools.Tools.WallPaper
{
    /// <summary>
    /// WallPaper.xaml 的交互逻辑
    /// </summary>
    public partial class MainWallPaper : Page
    {
        public MainWallPaper()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        { 

        }
        private void cboxStartOn_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cboxStartOn.IsChecked)
            {
                //
            }
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            BackGround backGround = new BackGround(tboxFileInfomation.Text,tboxDate.Text);
            backGround.Show();
        }
    }
}