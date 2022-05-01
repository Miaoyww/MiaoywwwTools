using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MiaoywwwTools
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public class MessageResult
    {
        /// <summary>
        /// 结果，Yes为true，No为false
        /// </summary>
        public bool IsYes { get; set; }
    }

    public class MessageBoxEventArgs : EventArgs
    {
        /// <summary>
        /// 结果，Yes为true，No为false
        /// </summary>
        public MessageResult Result { get; set; }
    }

    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox : Window
    {
        public event EventHandler<MessageBoxEventArgs> Result;

        public string Context
        {
            get { return TextBlock_MessageBody.Text; }
            set { TextBlock_MessageBody.Text = value; }
        }

        private bool _isLegal = false;

        public MessageBox()
        {
            InitializeComponent();
        }

        public static void Show(string context, EventHandler<MessageBoxEventArgs> result)
        {
            var mb = new MessageBox();
            mb.Context = context;
            mb.Result += result;
            mb.Show();
        }

        public static MessageResult ShowDialog(string context)
        {
            var mb = new MessageBox();
            mb.Context = context;
            MessageResult r = null;
            mb.Result += (s, e) =>
            {
                r = e.Result;
            };
            mb.ShowDialog();
            return r;
        }

        private void CloseWindow()
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Close(); };
                story.Begin(this);
            }
        }

        private void Btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            _isLegal = true;
            CloseWindow();
            Result?.Invoke(this, new MessageBoxEventArgs()
            {
                Result = new MessageResult()
                {
                    IsYes = true
                }
            });
        }

        private void Btn_No_Click(object sender, RoutedEventArgs e)
        {
            _isLegal = true;
            CloseWindow();
            Result?.Invoke(this, new MessageBoxEventArgs()
            {
                Result = new MessageResult()
                {
                    IsYes = false
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isLegal)
            {
                e.Cancel = false;
                var story = (Storyboard)this.Resources["ShowWindow"];
                if (story != null)
                {
                    story.Begin(this);
                }
            }
        }

        private void Border_Top_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}