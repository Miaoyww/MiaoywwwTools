using System;
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
            if(e.Args.Length >= 1)
            {
                if (e.Args[0] == "Updataed")
                {
                    MessageBox.ShowDialog($"更新完成！当前版本{GlobalV.AppVersion_ver}");
                }
            }

            base.OnStartup(e);
        }
    }
}