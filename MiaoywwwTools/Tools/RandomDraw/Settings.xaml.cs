using Microsoft.Win32;
using RandomDrawLib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiaoywwwTools.Tools.RandomDraw
{
    /// <summary>
    /// ToolRrSettings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        public RaDraw raDraw = new();
        public Border changeColorBorder;
        public TextBox changeContextBox;

        public int TNchangetimes;
        public int TGchangetimes;
        public int CNchangetimes;
        public int CGchangetimes;
        public int CBchangetimes;
        public bool AnimationCompleted = true;

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
            if (Random_Mode.IsChecked is true)
            {
                Registry.SetValue(raDraw.keypath, "Mode", "random");
                Grade_Mode.IsChecked = false;
            }
        }

        private void Grade_Mode_Checked(object sender, RoutedEventArgs e)
        {
            /*
            if (Random_Mode.IsChecked == true)
            {
                Random_Mode.IsChecked = false;
            }
            */
            MessageBox.ShowDialog("功能暂未开放");
            Grade_Mode.IsChecked = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 判断注册表是否有指定值
            // 无则创建
            string[][] keylist = new string[6][];
            for (int i = 0; i < keylist.Length; i++)
            {
                keylist[i] = new string[2];
            }
            keylist[0] = new string[] { "Mode", "random" };
            keylist[1] = new string[] { "NameColor", "#FFFFFFFF" };
            keylist[2] = new string[] { "NameSize", "340" };
            keylist[3] = new string[] { "GradeColor", "#FFFFFFFF" };
            keylist[4] = new string[] { "GradeSize", "100" };
            keylist[5] = new string[] { "BackGroundColor", "#FF000000" };
            foreach (string[] item in keylist)
            {
                object? keyvalue = Registry.GetValue(raDraw.keypath, item[0], null);
                if (keyvalue is null)
                {
                    Registry.SetValue(raDraw.keypath, item[0], item[1]);
                }
            }
            // 判断是否为True，是则控制Random_Mode为勾选状态
            if (Registry.GetValue(raDraw.keypath, "Mode", false)?.ToString() == "random")
            {
                Random_Mode.IsChecked = true;
            }
            TextBox_NameSettings_Color.Text = Registry.GetValue(raDraw.keypath, "NameColor", false)?.ToString();
            TextBox_GradeSettings_Color.Text = Registry.GetValue(raDraw.keypath, "GradeColor", false)?.ToString();
            TextBox_BackGroundSettings_Color.Text = Registry.GetValue(raDraw.keypath, "BackGroundColor", false)?.ToString();
            ChangeSettingsBorderColor();
            TextBox_NameSettings_Size.Text = Registry.GetValue(raDraw.keypath, "NameSize", false)?.ToString();
            TextBox_GradeSettings_Size.Text = Registry.GetValue(raDraw.keypath, "GradeSize", false)?.ToString();
        }

        private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void FontSizeContentLegality(TextBox textBox)
        {
            if (textBox.Text != "")
            {
                try
                {
                    int.Parse(textBox.Text);
                }
                catch (FormatException)
                {
                    MessageBox.ShowDialog("请输入一个正整数");
                    textBox.Text = "100";
                    return;
                }
                if (int.Parse(textBox.Text) <= 0)
                {
                    MessageBox.ShowDialog("请输入一个正整数");
                    textBox.Text = "100";
                }
            }
            else
            {
                MessageBox.ShowDialog("请输入一个正整数");
                textBox.Text = "100";
            }
        }

        private void ColorContentLegality(TextBox textBox)
        {
            BrushConverter brushConverter = new BrushConverter();
            if (textBox.Text != "")
            {
                try
                {
                    Brush test = (Brush)brushConverter.ConvertFromString(textBox.Text);
                }
                catch (FormatException)
                {
                    MessageBox.ShowDialog("请输入一个正确的ARGB颜色");
                    textBox.Text = "#FFFFFFFF";
                }
            }
            else
            {
                MessageBox.ShowDialog("请输入一个正确的ARGB颜色");
                textBox.Text = "#FFFFFFFF";
            }
        }

        // 颜色选择
        private void Btn_SelectName_Click(object sender, RoutedEventArgs e)
        {
            changeColorBorder = Border_NameSettings;
            changeContextBox = TextBox_NameSettings_Color;
        }

        private void Btn_SelectGrade_Click(object sender, RoutedEventArgs e)
        {
            changeColorBorder = Border_GradeSettings;
            changeContextBox = TextBox_GradeSettings_Color;
        }

        private void Btn_SelectBackGround_Click(object sender, RoutedEventArgs e)
        {
            changeColorBorder = Border_BackGroundSettings;
            changeContextBox = TextBox_BackGroundSettings_Color;
        }

        /// <summary>
        /// 以组件内容修改其颜色
        /// </summary>
        public void ChangeSettingsBorderColor()
        {
            BrushConverter brushConverter = new BrushConverter();
            Border_BackGroundSettings.Background = (Brush)brushConverter.ConvertFromString(TextBox_BackGroundSettings_Color.Text);
            Border_GradeSettings.Background = (Brush)brushConverter.ConvertFromString(TextBox_GradeSettings_Color.Text);
            Border_NameSettings.Background = (Brush)brushConverter.ConvertFromString(TextBox_NameSettings_Color.Text);
        }
        // 设置重置
        private void Btn_OutPutSettingsReSet_Click(object sender, RoutedEventArgs e)
        {
            TextBox_NameSettings_Color.Text = TextBox_GradeSettings_Color.Text = "#FFFFFFFF";
            TextBox_BackGroundSettings_Color.Text = "#FF000000";
            TextBox_NameSettings_Size.Text = (340).ToString();
            TextBox_GradeSettings_Size.Text = (100).ToString();
            ChangeSettingsBorderColor();
        }
        public void SaveSettings(){
            // 颜色
            ColorContentLegality(TextBox_NameSettings_Color);
            ColorContentLegality(TextBox_GradeSettings_Color);
            ColorContentLegality(TextBox_BackGroundSettings_Color);
            ChangeSettingsBorderColor();
            Registry.SetValue(raDraw.keypath, "BackGroundColor", TextBox_BackGroundSettings_Color.Text);
            Registry.SetValue(raDraw.keypath, "GradeColor", TextBox_GradeSettings_Color.Text);
            Registry.SetValue(raDraw.keypath, "NameColor", TextBox_NameSettings_Color.Text);

            // 字体大小
            FontSizeContentLegality(TextBox_NameSettings_Size);
            FontSizeContentLegality(TextBox_GradeSettings_Size);
            Registry.SetValue(raDraw.keypath, "NameSize", TextBox_NameSettings_Size.Text);
            Registry.SetValue(raDraw.keypath, "GradeSize", TextBox_GradeSettings_Size.Text);
        }

        // 设置保存
        private void Btn_OutPutSettingsSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        // 拾色器确认
        private void ColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
            if (changeColorBorder is not null && changeContextBox is not null)
            {
                changeColorBorder.Background = ColorPicker_Main.SelectedBrush;
                changeContextBox.Text = ColorPicker_Main.SelectedBrush.ToString();
                SaveSettings();
            }
        }

        // 拾色器的取消按钮，关闭窗口
        private void ColorPicker_Main_Canceled(object sender, EventArgs e)
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Close(); };
                story.Begin(this);
            }
        }
    }
}