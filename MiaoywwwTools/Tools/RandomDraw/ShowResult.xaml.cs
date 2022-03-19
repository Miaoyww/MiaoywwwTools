using Microsoft.Win32;
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
        public string keypath = "HKEY_CURRENT_USER\\SOFTWARE\\Miaoywww\\MiaoywwwTools\\Tools\\RandomDraw\\";

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
                raDraw.Read();
                string[] result = raDraw.GetRandomResult();
                Label_Name.Content = result[0];
                Label_Grade.Content = result[1];
            }
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

            BrushConverter brushConverter = new BrushConverter();
            Label_Name.Foreground = (Brush)brushConverter.ConvertFromString(Registry.GetValue(keypath, "NameColor", null).ToString());
            Label_Grade.Foreground = (Brush)brushConverter.ConvertFromString(Registry.GetValue(keypath, "GradeColor", null).ToString());
            Grid_Main.Background = (Brush)brushConverter.ConvertFromString(Registry.GetValue(keypath, "BackGround", null).ToString());

            Label_Name.FontSize = int.Parse(Registry.GetValue(keypath, "NameSize", null).ToString());
            Label_Grade.FontSize = int.Parse(Registry.GetValue(keypath, "GradeSize", null).ToString());
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            WinMain.winMain.Login = false;
            this.Close();
        }
    }
}