using System;
using System.Threading;
using System.Diagnostics;

namespace Calculator.UI
{
    class Program
    {
        static Mutex mutexObj = new Mutex();
        static int x = 0;

        static void Main(string[] args)
        {
            for(int i = 0; i<5; i++)
            {
                var myThread = new Thread(Count);
            }

            Console.ReadKey();
        }

        private static void Count()
        {
            mutexObj.WaitOne();
            x = 1;

            for(var i = 1; i<9; i++)
            {

            }
        }
    }
}
