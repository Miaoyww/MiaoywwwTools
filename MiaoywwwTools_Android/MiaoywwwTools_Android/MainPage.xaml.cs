using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MiaoywwwTools_Android
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public class StoppableTimer
        {
            private readonly TimeSpan timespan;
            private readonly Action callback;

            private CancellationTokenSource cancellation;

            public StoppableTimer(TimeSpan timespan, Action callback)
            {
                this.timespan = timespan;
                this.callback = callback;
                this.cancellation = new CancellationTokenSource();
            }

            public void Start()
            {
                CancellationTokenSource cts = this.cancellation; // safe copy
                Device.StartTimer(this.timespan,
                    () => {
                        if (cts.IsCancellationRequested) return false;
                        this.callback.Invoke();
                        return true; // or true for periodic behavior
                    });
            }

            public void Stop()
            {
                Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
            }

            public void Dispose()
            {

            }
        }
        /*
         From https://stackoverflow.com/questions/41586553/how-to-cancel-a-timer-before-its-finished
         */

        public JObject HttpGetJson(string url)
        {
            HttpClient httpClientV = new HttpClient();
            httpClientV.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            var downloadV = httpClientV.GetStringAsync(url).Result;

            JObject jsonContentV = (JObject)JsonConvert.DeserializeObject(downloadV);
            return jsonContentV;
        }

        public JObject JObj_HitokotoJson;
        public string Str_HitokotoContent;
        public string Str_HitokotoFrom;
        public string Str_HitokotoFromWho;

        public async Task Hitokoto()
        {
            try
            {
                JObj_HitokotoJson = HttpGetJson("https://v1.hitokoto.cn/");
                Str_HitokotoContent = JObj_HitokotoJson["hitokoto"].ToString();
                Str_HitokotoFrom = JObj_HitokotoJson["from"].ToString();
                Str_HitokotoFromWho = JObj_HitokotoJson["from_who"].ToString();
            }
            catch (Exception)
            {
            }
            HitokotoContent.Text = $"{Str_HitokotoContent}";
            if (Str_HitokotoFromWho != "")
            {
                HitokotoFrom.Text = $"-{Str_HitokotoFrom} / {Str_HitokotoFromWho}";
            }
            else
            {
                HitokotoFrom.Text = $"-{Str_HitokotoFrom}";
            }
        }
        private StoppableTimer stoppableTimer;

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (stoppableTimer != null)
            {
                stoppableTimer.Stop();
            }
            // await DisplayAlert("","你好啊！","(●'◡'●)");
            await Hitokoto();
        }

        private async void AutoHi_Clicked(object sender, EventArgs e)
        {
            if (stoppableTimer != null)
            {
                stoppableTimer.Stop();
            }
            stoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(int.Parse(TimeInterval.Text)), async () =>
            {
                await Hitokoto();
            });
            stoppableTimer.Start();
        }

        private async void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
                int interval = (int)TimeIntervalS.Value;
                TimeInterval.Text = interval.ToString();
        }
    }
}