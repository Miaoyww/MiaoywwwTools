using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MiaoywwwTools.Tools.RandomDraw
{
    /// <summary>
    /// Compute.xaml 的交互逻辑
    /// </summary>
    public partial class Compute : Window
    {
        public Compute()
        {
            InitializeComponent();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Close(); };
                story.Begin(this);
            }
        }

        private void TopBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Btn_Compute_Click(object sender, RoutedEventArgs e)
        {
            if (RealTime.IsChecked is true)
            {
                Thread thread = new Thread(new ThreadStart(ThreadList));
                thread.Start();
            }
            else
            {
                Thread threadb = new Thread(new ThreadStart(AfterComputeResult));
                threadb.Start();
            }
        }

        private void ThreadList()
        {
            for (int i = 0; i < 1; i++)
                new Thread(() => { RealTimeComputeResult(); }).Start();
        }

        private void AfterComputeResult()
        {
            RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
            raDraw.Read();
            int arraylength = raDraw.GetStdataLentgh();
            string[,] std = new string[arraylength, 2];  // 数据
            JObject jsonObject = raDraw.GetStdataContent();
            int times = 0;
            // 委托主线程改变内容
            TextBox_ComputeTimes.Dispatcher.Invoke(new Action(
                delegate
                {
                    times = int.Parse(TextBox_ComputeTimes.Text);
                })
                );
            // 将jsonObject中的数据转存到std中，并加入一个“重复到的次数”
            for (int i = 0; i < arraylength; i++)
            {
                std[i, 0] = jsonObject[i.ToString()]["name"].ToString();
                std[i, 1] = "0";
            }
            // 重复执行
            for (int i = times; i >= 0; i--)
            {
                TextBox_RemanentTimes.Dispatcher.Invoke(new Action(
                delegate
                {
                    TextBox_RemanentTimes.Text = i.ToString();
                })
                );
                raDraw.Read();
                string[] result = raDraw.GetRandomResult();
                std[int.Parse(result[2]), 1] = (int.Parse(std[int.Parse(result[2]), 1]) + 1).ToString();
            }
            // 用冒泡排序将二维数组排序
            for (int i = 0; i < arraylength; i++)
            {
                for (int j = 0; j < arraylength - i - 1; j++)
                {
                    if (int.Parse(std[j, 1]) < int.Parse(std[j + 1, 1]))
                    {
                        string[] temp0 = new string[2];
                        string[] temp1 = new string[2];
                        temp0[0] = std[j, 0];  // 人名
                        temp0[1] = std[j, 1]; // 成绩

                        temp1[0] = std[j + 1, 0];
                        temp1[1] = std[j + 1, 1];

                        std[j, 0] = temp1[0];
                        std[j, 1] = temp1[1];

                        std[j + 1, 0] = temp0[0];
                        std[j + 1, 1] = temp0[1];
                    }
                }
            }
            DateTime time = new DateTime();
            time = DateTime.Now;
            int second = time.Second;
            string filepath = String.Format(
                "./Result/ToolsRr_compute_{0}.{1}.{2}.{3}_{4}_{5}.txt",
                time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second
            );
            if (Directory.Exists("./Result/") is false)
            {
                Directory.CreateDirectory("Result");
            }
            File.Create(filepath).Close();
            StreamWriter sw = new(filepath, true, System.Text.Encoding.Default);
            for (int i = 0; i < arraylength; i++)
            {
                sw.WriteLine(String.Format("-{0} {1}:{2}", i, std[i, 0], std[i, 1]));
            }
            sw.Flush();
            sw.Close();
        }

        private void RealTimeComputeResult()
        {
            RandomDrawLib.RaDraw raDraw = new RandomDrawLib.RaDraw();
            raDraw.Read();
            int arraylength = raDraw.GetStdataLentgh();
            string[,] std = new string[arraylength, 2];  // 数据
            JObject jsonObject = raDraw.GetStdataContent();
            int times = 0;
            // 委托主线程改变内容
            TextBox_ComputeTimes.Dispatcher.Invoke(new Action(
                delegate
                {
                    times = int.Parse(TextBox_ComputeTimes.Text);
                })
                );
            // 将jsonObject中的数据转存到std中，并加入一个“重复到的次数”
            for (int i = 0; i < arraylength; i++)
            {
                std[i, 0] = jsonObject[i.ToString()]["name"].ToString();
                std[i, 1] = "0";
            }
            // 重复执行
            for (int i = times; i >= 0; i--)
            {
                TextBox_RemanentTimes.Dispatcher.Invoke(new Action(
                delegate
                {
                    TextBox_RemanentTimes.Text = i.ToString();
                })
                );
                raDraw.Read();
                string[] result = raDraw.GetRandomResult();
                std[int.Parse(result[2]), 1] = (int.Parse(std[int.Parse(result[2]), 1]) + 1).ToString();
                // 用冒泡排序将二维数组排序
                for (int ib = 0; ib < arraylength; ib++)
                {
                    for (int j = 0; j < arraylength - ib - 1; j++)
                    {
                        if (int.Parse(std[j, 1]) < int.Parse(std[j + 1, 1]))
                        {
                            string[] temp0 = new string[2];
                            string[] temp1 = new string[2];
                            temp0[0] = std[j, 0];  // 人名
                            temp0[1] = std[j, 1]; // 成绩

                            temp1[0] = std[j + 1, 0];
                            temp1[1] = std[j + 1, 1];

                            std[j, 0] = temp1[0];
                            std[j, 1] = temp1[1];

                            std[j + 1, 0] = temp0[0];
                            std[j + 1, 1] = temp0[1];
                        }
                    }
                }
                TextBlock_Result.Dispatcher.Invoke(new Action(
                delegate
                {
                    TextBlock_Result.Text = null;
                })
                );
                for (int ic = 0; ic < arraylength; ic++)
                {
                    TextBlock_Result.Dispatcher.Invoke(new Action(
                    delegate
                    {
                        TextBlock_Result.Text += string.Format("-{0} {1}:{2} \n", ic, std[ic, 0], std[ic, 1]);
                    })
                    );
                }
            }
        }

        private void TextBox_ComputeTimes_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int.Parse(TextBox_ComputeTimes.Text);
            }
            catch
            {
                WinMessage winMessage = new WinMessage();
                winMessage.SetMessage("错误", "请输入一个正整数", "close", "yesno");
                return;
            }
            if (int.Parse(TextBox_ComputeTimes.Text) <= 0)
            {
                WinMessage winMessage = new WinMessage();
                winMessage.SetMessage("错误", "请输入一个正整数", "close", "yesno");
            }
        }
    }
}