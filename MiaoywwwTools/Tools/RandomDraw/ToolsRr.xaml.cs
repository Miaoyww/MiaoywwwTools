using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using RandomDrawLib;
using MiaoywwwTools.Tools.RandomDraw;

namespace MiaoywwwTools
{
    /// <summary>
    /// ToolsRr.xaml 的交互逻辑
    /// </summary>
    public partial class ToolsRr : Page
    {
        public ToolsRr trr;

        public ToolsRr()
        {
            InitializeComponent();
            trr = this;
        }
        private void Btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
            string[] result = raDraw.GetRandomResult();
            if(result != null)
            {
                ShowResult showResult = new ShowResult();
                showResult.Label_Name.Content = result[0];
                showResult.Label_Grade.Content = result[1];
                showResult.Show();
            }
        }

        private void Btn_Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ToolRrSettings toolRrSettings = new ToolRrSettings();
            toolRrSettings.ShowDialog();
        }
    }
}