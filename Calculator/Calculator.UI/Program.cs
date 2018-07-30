using System;
using System.Diagnostics;

namespace Calculator.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = Process.GetCurrentProcess();

            Console.WriteLine(process.ProcessName);

            Console.ReadKey();
        }
    }
}
