using Microsoft.Win32;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiaoywwwTools
{
    /// <summary>
    /// ToolRrSettings.xaml 的交互逻辑
    /// </summary>
    public partial class ToolRrSettings : Window
    {
        public ToolRrSettings()
        {
            InitializeComponent();
        }

        public string keypath = "HKEY_CURRENT_USER\\SOFTWARE\\Miaoywww\\MiaoywwwTools\\ToolsRr";
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

        private void Random_Mode_Checked(object sender, RoutedEventArgs e)
        {
           if (Grade_Mode.IsChecked == true)
            {
                Grade_Mode.IsChecked = false;
            }
            Registry.SetValue(keypath, "Mode", "random");
        }

        private void Grade_Mode_Checked(object sender, RoutedEventArgs e)
        {
            /*
            if (Random_Mode.IsChecked == true)
            {
                Random_Mode.IsChecked = false;
            }
            */
            WinMessage winMessage = new WinMessage();
            winMessage.SetMessage("注意", "功能暂未开放", "close", "yes");
            winMessage.ShowDialog();
            Grade_Mode.IsChecked = false;

        }

        private void GlobalShortcuts_Checked(object sender, RoutedEventArgs e)
        {
            WinMessage winMessage = new WinMessage();
            winMessage.SetMessage("注意", "功能暂未开放", "close", "yes");
            winMessage.ShowDialog();
            GlobalShortcuts.IsChecked = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 判断是否为True，是则控制Random_Mode为勾选状态
            if (Registry.GetValue(keypath, "Mode", false).ToString() == "random")
            {
                Random_Mode.IsChecked = true;
            }
        }

        private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
