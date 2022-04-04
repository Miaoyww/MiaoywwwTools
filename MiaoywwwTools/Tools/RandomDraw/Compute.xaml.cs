using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        public bool Computing;
        public bool AnimationCompleted = true;
        public bool _run = true;
        public Thread thread;

        public Compute()
        {
            InitializeComponent();
        }

        public static void Show()
        {
            Compute compute = new();
            compute.ShowDialog();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate
                {
                    this.Close();
                };
                story.Begin(this);
            }
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

        private void Btn_Compute_Click(object sender, RoutedEventArgs e)
        {
            if (Computing)
            {
                Btn_Compute.Content = "开始";
                _run = false;
                Computing = false;
            }
            else
            {
                RandomDrawLib.RaDraw raDraw = new();
                JObject read = raDraw.Read();  // 若文件不存在，返回false
                if (read is not null)
                {
                    Btn_Compute.Content = "停止";
                    Computing = true;
                    _run = true;
                    int runtimes = int.Parse(TextBox_ComputeTimes.Text);
                    bool realTimeoutput = (bool)RealTime.IsChecked;
                    thread = new Thread(() =>
                    {
                        GetRandomResult(runtimes, realTimeoutput);
                    });
                    // thread.SetApartmentState(ApartmentState.STA);
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
        }

        private void Story_Completed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public async void GetRandomResult(int RunTimes, bool RealTimeOutPut = false)
        {
            RandomDrawLib.RaDraw raDraw = new();
            JObject read = raDraw.Read();
            if (read is not null)
            {
                int arraylength = raDraw.GetStdataLentgh();
                string[,] PersonList = new string[arraylength, 2];  // 数据
                JObject jsonObject = raDraw.Read();
                // 将jsonObject中的数据转存到std中，并加入一个“重复到的次数”
                for (int i = 0; i < arraylength; i++)
                {
                    PersonList[i, 0] = jsonObject[i.ToString()]["name"].ToString();
                    PersonList[i, 1] = "0";
                }
                // 重复执行
                for (int lefttimes = RunTimes; RunTimes >= 0; RunTimes--)
                {
                    if (_run)
                    {
                        await this.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            this.TextBox_RemanentTimes.Text = RunTimes.ToString();
                        }));
                        JObject reader = raDraw.Read();
                        JObject result = raDraw.GetRandomResult(reader);
                        PersonList[int.Parse(result["select"].ToString()), 1] = (int.Parse(PersonList[int.Parse(result["select"].ToString()), 1]) + 1).ToString();
                        PersonList = await SortData(PersonList, arraylength);
                        if (RealTimeOutPut is true)
                        {
                            string wholeContent = "";
                            for (int ic = 0; ic < arraylength; ic++)
                            {
                                wholeContent += string.Format("-{0} {1}:{2} \n", ic, PersonList[ic, 0], PersonList[ic, 1]);
                            }

                            await this.Dispatcher.BeginInvoke(new Action(delegate
                            {
                                this.TextBlock_Result.Text = wholeContent;
                            }));
                        }
                    }
                    else
                    {
                        return;
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
                    sw.WriteLine(String.Format("-{0} {1}:{2}", i, PersonList[i, 0], PersonList[i, 1]));
                }
                sw.Flush();
                sw.Close();
                if (RealTimeOutPut is false)
                {
                    Process.Start("notepad.exe", filepath);
                }
                Computing = false;
                this.Dispatcher.Invoke(new Action(delegate
                {
                    this.Btn_Compute.Content = "开始";
                }));
            }
            else
            {
                return;
            }
        }

        private Task<string[,]> SortData(string[,] data, int length)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length - i - 1; j++)
                    {
                        if (int.Parse(data[j, 1]) < int.Parse(data[j + 1, 1]))
                        {
                            string[] temp0 = new string[2];
                            string[] temp1 = new string[2];
                            temp0[0] = data[j, 0];  // 人名
                            temp0[1] = data[j, 1]; // 成绩

                            temp1[0] = data[j + 1, 0];
                            temp1[1] = data[j + 1, 1];

                            data[j, 0] = temp1[0];
                            data[j, 1] = temp1[1];

                            data[j + 1, 0] = temp0[0];
                            data[j + 1, 1] = temp0[1];
                        }
                    }
                }
                return data;
            });
        }

        private void TextBox_ComputeTimes_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int.Parse(TextBox_ComputeTimes.Text);
            }
            catch (FormatException)
            {
                ShowInfo("请输入一个正整数");
                TextBox_ComputeTimes.Text = (1000).ToString();
                return;
            }
            if (int.Parse(TextBox_ComputeTimes.Text) <= 0)
            {
                ShowInfo("请输入一个正整数");
                TextBox_ComputeTimes.Text = (1000).ToString();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Label_Info.Opacity = 0;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _run = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}