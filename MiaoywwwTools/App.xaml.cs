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
            base.OnStartup(e);
        }
    }
}