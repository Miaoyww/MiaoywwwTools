using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;

namespace MiaoywwwTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Threading.Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "AppName", out ret);//这里填写程序名称
            if (!ret)
            {
                Environment.Exit(0);
            }
            if (e.Args.Length >= 1)
            {
                if (e.Args[0] == "Updataed")
                {
                    try
                    {
                        StreamReader reader = File.OpenText(@"./version.json");
                        JObject jsonContent = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                        GlobalV.AppVersion_ver = jsonContent["MiaoywwwTools"]["version"].ToString();
                        GlobalV.AppVersion_time = jsonContent["MiaoywwwTools"]["time"].ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ShowDialog($"读取版本信息错误, {ex}");
                    }
                    MessageBox.ShowDialog($"更新完成！当前版本{GlobalV.AppVersion_ver}");
                }
            }

            base.OnStartup(e);
        }
    }
}