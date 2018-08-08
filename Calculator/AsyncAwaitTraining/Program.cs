using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace AsyncAwaitTraining
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Start Main");
            for (int i = 1; i < 6; i++)
            {
                Reader reader = new Reader(i);
            }

            Console.ReadLine();
        }
    }

    class Reader
    {
        // создаем семафор
        static Semaphore sem = new Semaphore(3, 3);
        Thread myThread;
        int count = 3;// счетчик чтения

        public Reader(int i)
        {
            myThread = new Thread(Read);
            myThread.Name = "Читатель " + i.ToString();
            myThread.Start();
        }

        public void Read()
        {
            while (count > 0)
            {
                sem.WaitOne();

                Console.WriteLine("{0} входит в библиотеку", Thread.CurrentThread.Name);

                Console.WriteLine("{0} читает", Thread.CurrentThread.Name);
                Thread.Sleep(1000);

                Console.WriteLine("{0} покидает библиотеку", Thread.CurrentThread.Name);

                sem.Release();

                count--;
                Thread.Sleep(1000);
            }
        }
    }
}
