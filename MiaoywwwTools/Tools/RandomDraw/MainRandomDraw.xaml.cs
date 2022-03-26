using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RandomDrawLib;

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
        public RaDraw raDraw = new();
        private void Btn_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            BrushConverter brushConverter = new BrushConverter();
            RandomDrawLib.RaDraw raDraw = new();
            raDraw.Read();
            string[] result = raDraw.GetRandomResult();
            if (result != null)
            {
                Thread thread = new(() =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        Brush nameColor = (Brush)brushConverter.ConvertFromString(Registry.GetValue(raDraw.keypath, "NameColor", null).ToString());
                        Brush gradeColor = (Brush)brushConverter.ConvertFromString(Registry.GetValue(raDraw.keypath, "GradeColor", null).ToString());
                        Brush backgroundColor = (Brush)brushConverter.ConvertFromString(Registry.GetValue(raDraw.keypath, "BackGroundColor", null).ToString());
                        int nameFontSize = int.Parse(Registry.GetValue(raDraw.keypath, "NameSize", null).ToString());
                        int gradeFontSize = int.Parse(Registry.GetValue(raDraw.keypath, "GradeSize", null).ToString());
                        ShowResult.Show(nameColor,
                            gradeColor,
                            backgroundColor,
                            nameFontSize,
                            gradeFontSize,
                            result[0],
                            result[1]);
                        
                    }));
                });
                thread.IsBackground = true;
                thread.Start();
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