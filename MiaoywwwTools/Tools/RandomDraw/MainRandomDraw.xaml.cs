using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using RandomDrawLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            RaDraw raDraw = new();
            JObject read = raDraw.Read();
            List<object>? result = new List<object>();
            JObject? randomResult = new();
            if (Registry.GetValue(raDraw.keypath, "Mode-Re", null)?.ToString() == "list")
            {
                result = raDraw.GetListResult(read);
                randomResult = (JObject)result[0];
            }
            else
            {
                result = raDraw.GetListResult(read);
                result[1] = null;
                randomResult = raDraw.GetRandomResult(read);
            }
            if (result != null)
            {
                Thread thread = new(() =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
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
                            randomResult["name"].ToString(),
                            randomResult["grade"].ToString(),
                            (JObject)result[1]);
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

    }
}