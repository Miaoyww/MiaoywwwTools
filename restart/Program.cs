using System.Diagnostics;

namespace restart
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int runtimes = 0;
            while (runtimes < 5)
            {
                Process[] ps = Process.GetProcessesByName(args[0]);

                if (ps.Length == 0)
                {
                    Process.Start(Environment.CurrentDirectory + @"\" + args[0] + ".exe");
                    break;
                }
                else
                {
                    // 防止程序未完全关闭导致重启失败
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                    }).Wait();
                    runtimes++;
                }
            }
        }
    }
}