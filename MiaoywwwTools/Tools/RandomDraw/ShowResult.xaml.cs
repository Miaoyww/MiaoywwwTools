using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        public JObject jObject;
        public string keypath = "HKEY_CURRENT_USER\\SOFTWARE\\Miaoywww\\MiaoywwwTools\\Tools\\RandomDraw\\";

        public static void Show(
            Brush nameColor,
            Brush gradeColor,
            Brush backgroundColor,
            int nameFontSize,
            int gradeFontSize,
            string name,
            string grade,
            JObject jobject)
        {
            ShowResult showResult = new();
            showResult.ChangeColor(nameColor, gradeColor, backgroundColor);
            showResult.ChangeFontSize(nameFontSize, gradeFontSize);
            showResult.Label_Name.Content = name;
            showResult.Label_Grade.Content = grade;
            if(jobject is not null)
            {
                showResult.jObject = jobject;
            }
            showResult.ShowDialog();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
                if(jObject is not null)
                {
                    JObject randomResult = new();
                    List<object> result = new();
                    result = raDraw.GetListResult(jObject);
                    if (result != null)
                    {
                        Label_NextList.Opacity = 0;
                        randomResult = (JObject)result[0];
                        jObject = (JObject)result[1];
                        Label_Name.Content = randomResult["name"].ToString();
                        Label_Grade.Content = randomResult["grade"].ToString();
                    }
                    else
                    {
                        Label_NextList.Opacity = 1;
                        JObject read = raDraw.Read();
                        List<object> reResult = raDraw.GetListResult(read);
                        JObject reRandomResult = (JObject)reResult[0];
                        jObject = (JObject)reResult[1];
                        Label_Name.Content = reRandomResult["name"].ToString();
                        Label_Grade.Content = reRandomResult["grade"].ToString();
                    }
                }
                else
                {
                    JObject read = raDraw.Read();
                    JObject? result = raDraw.GetRandomResult(read);
                    Label_Name.Content = result["name"]?.ToString();
                    Label_Grade.Content = result["grade"]?.ToString();
                }

            }
        }

        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="grade">成绩</param>
        /// <param name="background">背景</param>
        public void ChangeColor(Brush name, Brush grade, Brush background)
        {
            Label_Name.Foreground = name;
            Label_Grade.Foreground = grade;
            Grid_Main.Background = background;
        }

        /// <summary>
        /// 改变字体大小
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="grade">名称</param>
        public void ChangeFontSize(int name, int grade)
        {
            Label_Name.FontSize = name;
            Label_Grade.FontSize = grade;
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
            GlobalV.Login = false;
            this.Close();
        }
    }
}