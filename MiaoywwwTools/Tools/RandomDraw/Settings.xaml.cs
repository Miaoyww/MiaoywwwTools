using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

        public string keypath = "HKEY_CURRENT_USER\\SOFTWARE\\Miaoywww\\MiaoywwwTools\\Tools\\RandomDraw\\";
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
                Registry.SetValue(keypath, "Mode", "random");
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
            WinMessage winMessage = new();
            winMessage.SetMessage("注意", "功能暂未开放", "close", "yes");
            winMessage.ShowDialog();
            Grade_Mode.IsChecked = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 判断注册表是否有指定值
            // 无则创建
            if (Registry.GetValue(keypath, "Mode", null) == null)
            {
                Registry.SetValue(keypath, "Mode", "random");
            }
            if (Registry.GetValue(keypath, "FaceCleanUp", null) == null)
            {
                Registry.SetValue(keypath, "FaceCleanUp", "false");
            }

            if (Registry.GetValue(keypath, "NameColor", null) == null)
            {
                Registry.SetValue(keypath, "NameColor", "#FFFFFFFF");
            }
            if (Registry.GetValue(keypath, "NameSize", null) == null)
            {
                Registry.SetValue(keypath, "NameSize", "340");
            }


            if (Registry.GetValue(keypath, "GradeColor", null) == null)
            {
                Registry.SetValue(keypath, "GradeColor", "#FFFFFFFF");
            }
            if (Registry.GetValue(keypath, "GradeSize", null) == null)
            {
                Registry.SetValue(keypath, "GradeSize", "100");
            }


            if (Registry.GetValue(keypath, "BackGround", null) == null)
            {
                Registry.SetValue(keypath, "BackGround", "#FF000000");
            }
            // 判断是否为True，是则控制Random_Mode为勾选状态
            if (Registry.GetValue(keypath, "Mode", false).ToString() == "random")
            {
                Random_Mode.IsChecked = true;
            }
            TextBox_NameSettings_Color.Text = Registry.GetValue(keypath, "NameColor", false).ToString();
            TextBox_GradeSettings_Color.Text = Registry.GetValue(keypath, "GradeColor", false).ToString();
            TextBox_BackGroundSettings_Color.Text = Registry.GetValue(keypath, "BackGround", false).ToString();

            TextBox_NameSettings_Size.Text = Registry.GetValue(keypath, "NameSize", false).ToString();
            TextBox_GradeSettings_Size.Text = Registry.GetValue(keypath, "GradeSize", false).ToString();

        }

        private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ShowInfo(string content)
        {
            if (AnimationCompleted)
            {
                Label_Info.Content = content;
                AnimationCompleted = false;
                var story = (Storyboard)this.Resources["ShowLabel"];
                if (story != null)
                {
                    story.Completed += delegate
                    {
                        var story = (Storyboard)this.Resources["HideLabel"];
                        if (story != null)
                        {
                            AnimationCompleted = true;
                            story.Begin(Label_Info, true);
                        }
                    };
                    story.Begin(Label_Info, true);
                }
            }
        }
        private void AAContentLegality(TextBox textBox)
        {
            if (textBox.Text != "")
            {
                try
                {
                    int.Parse(textBox.Text);
                }
                catch (FormatException)
                {
                    ShowInfo("请输入一个正整数");
                    textBox.Text = "100";
                    return;
                }
                if (int.Parse(textBox.Text) <= 0)
                {
                    ShowInfo("请输入一个正整数");
                    textBox.Text = "100";
                }
            }
        }
        private void FFContentLegality(TextBox textBox)
        {
            BrushConverter brushConverter = new BrushConverter();
            if (textBox.Text != "")
            {
                try
                {
                    Brush a = (Brush)brushConverter.ConvertFromString(textBox.Text);
                }
                catch (FormatException)
                {
                    ShowInfo("请输入一个正确的ARGB颜色");
                    textBox.Text = "#FFFFFFFF";
                }
            }
            else
            {
                ShowInfo("请输入一个正确的ARGB颜色");
                textBox.Text = "#FFFFFFFF";
            }
        }
        // 字体设置
        private void TextBox_NameSettings_Size_TextChanged(object sender, TextChangedEventArgs e)
        {
            AAContentLegality(TextBox_NameSettings_Size);
            if (Registry.GetValue(keypath, "NameSize", null) != TextBox_NameSettings_Size.Text) 
            {
                if (TNchangetimes > 0)
                {
                    Registry.SetValue(keypath, "NameSize", TextBox_NameSettings_Size.Text);
                }
            }
            TNchangetimes++;
        }

        private void TextBox_GradeSettings_Size_TextChanged(object sender, TextChangedEventArgs e)
        {
            AAContentLegality(TextBox_GradeSettings_Size);
            if (Registry.GetValue(keypath, "GradeSize", null) != TextBox_GradeSettings_Size.Text)
            {
                if (TGchangetimes > 0)
                {
                    Registry.SetValue(keypath, "GradeSize", TextBox_GradeSettings_Size.Text);
                }
            }
            TGchangetimes++;

        }
        // 颜色设置
        private void TextBox_NameSettings_Color_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FFContentLegality(TextBox_NameSettings_Color);
            BrushConverter brushConverter = new BrushConverter();
            Border_NameSettings.Background = (Brush)brushConverter.ConvertFromString(TextBox_NameSettings_Color.Text);
            if (Registry.GetValue(keypath, "NameColor", null) != TextBox_NameSettings_Color.Text)
            {
                if (CNchangetimes > 0)
                    Registry.SetValue(keypath, "NameColor", TextBox_NameSettings_Color.Text);
            }
            CNchangetimes++;
        }

        private void TextBox_GradeSettings_Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            FFContentLegality(TextBox_GradeSettings_Color);

            BrushConverter brushConverter = new BrushConverter();
            Border_GradeSettings.Background = (Brush)brushConverter.ConvertFromString(TextBox_GradeSettings_Color.Text);
            if (Registry.GetValue(keypath, "GradeColor", null) != TextBox_GradeSettings_Color.Text)
            {
                if (CGchangetimes > 0)
                    Registry.SetValue(keypath, "GradeColor", TextBox_GradeSettings_Color.Text);
            }
            CGchangetimes++;
        }

        private void TextBox_BackGroundSettings_Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            FFContentLegality(TextBox_BackGroundSettings_Color);
            BrushConverter brushConverter = new BrushConverter();
            Border_BackGroundSettings.Background = (Brush)brushConverter.ConvertFromString(TextBox_BackGroundSettings_Color.Text);
            if (Registry.GetValue(keypath, "BackGround", null) != TextBox_BackGroundSettings_Color.Text)
            {
                if (CBchangetimes > 0)
                    Registry.SetValue(keypath, "BackGround", TextBox_BackGroundSettings_Color.Text);
            }
            CBchangetimes++;
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
        private void ColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
            if(changeColorBorder is not null)
            {
                changeColorBorder.Background = ColorPicker_Main.SelectedBrush;
            }
            if(changeContextBox is not null)
            {
                changeContextBox.Text = ColorPicker_Main.SelectedBrush.ToString();
            }
            
        }

        private void Btn_OutPutSettingsReSet_Click(object sender, RoutedEventArgs e)
        {
            TextBox_NameSettings_Color.Text = TextBox_GradeSettings_Color.Text = "#FFFFFF";
            TextBox_NameSettings_Size.Text = (340).ToString();
            TextBox_GradeSettings_Size.Text = (100).ToString();
        }

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