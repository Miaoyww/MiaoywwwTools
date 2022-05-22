using HandyControl.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace MiaoywwwTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Threading.Mutex? mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "AppName", out ret);//这里填写程序名称
            if (!ret)
            {
                Environment.Exit(0);
            }

            try
            {
                StreamReader reader = File.OpenText(@"./version.json");
                JObject jsonContent = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                GlobalV.AppVersion_ver = jsonContent["MiaoywwwTools"]["version"].ToString();
                GlobalV.AppVersion_time = jsonContent["MiaoywwwTools"]["time"].ToString();
                if (e.Args.Length >= 1)
                {
                    if (e.Args[0] == "updateed")
                    {

                        MessageBox.ShowDialog($"更新完成！当前版本{GlobalV.AppVersion_ver}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog($"读取版本信息错误, {ex}");
            }


            if (bool.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Miaoywww\MiaoywwwTools\", "CheckUpdateOnStart", "false").ToString()))
            {
                Thread check = new(() =>
                {
                    // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
                    
                });
            }
            base.OnStartup(e);
        }
    }
}