using System;

namespace RestartApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => Console.WriteLine()).Wait(4500);
            System.Diagnostics.Process.Start(args[0]);
        }
    }
}