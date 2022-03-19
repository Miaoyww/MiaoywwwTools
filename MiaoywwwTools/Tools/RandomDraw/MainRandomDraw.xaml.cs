using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MiaoywwwTools.Tools.RandomDraw
{
    /// <summary>
    /// ToolsRr.xaml 的交互逻辑
    /// </summary>
    public partial class MainRandomDraw : Page
    {
        public MainRandomDraw trr;

        public MainRandomDraw()
        {
            InitializeComponent();
            trr = this;
        }

        private void Btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RandomDrawLib.RaDraw raDraw = new();
            raDraw.Read();
            string[] result = raDraw.GetRandomResult();
            if (result != null)
            {
                ShowResult showResult = new();
                showResult.Label_Name.Content = result[0];
                showResult.Label_Grade.Content = result[1];
                showResult.Show();
            }
        }

        private void Btn_Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings settings = new();
            settings.ShowDialog();
        }

        private void Btn_ComputeProbability_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Thread thread = new(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { Compute.Show(); }));
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}