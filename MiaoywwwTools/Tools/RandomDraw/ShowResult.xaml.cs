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

namespace MiaoywwwTools.Tools.RandomDraw
{
    /// <summary>
    /// ShowResult.xaml 的交互逻辑
    /// </summary>
    public partial class ShowResult : Window
    {
        public ShowResult()
        {
            InitializeComponent();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
                string[] result = raDraw.GetRandomResult();
                Label_Name.Content = result[0];
                Label_Grade.Content = result[1];
            }
        }
    }
}
