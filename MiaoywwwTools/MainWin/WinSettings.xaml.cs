using Microsoft.Win32;
using System.Windows.Controls;

namespace MiaoywwwTools
{
    /// <summary>
    /// WinSettings.xaml 的交互逻辑
    /// </summary>
    public partial class WinSettings : Page
    {
        public WinSettings()
        {
            InitializeComponent();
        }

        private void Btn_CheckUpdata_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            /*
            object? keyvalue = Registry.GetValue("", "", null);
            if (keyvalue is null)
            {
                Registry.SetValue("", "", "");
            }*/
        }
    }
}