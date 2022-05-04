using System;
using System.Windows;

namespace update
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    GlobalV.unpack = Convert.ToBoolean(e.Args[0]);
                }
                catch (Exception ex)
                {
                    GlobalV.downloadurl = e.Args[0];
                }
            }
        }
    }
}