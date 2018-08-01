using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Calculator.UI
{
    public class Program
    {
        private const int MaxRows = 3;
        private const int MaxCols = 4000000;

        public static int[] ArrRowsSum = new int[MaxRows];
        public static int[][] MainArr = new int[MaxRows][];

        public static long Milliseconds { get; set; }

        private static void Main()
        {
            InitArray(MainArr);

            SumDefault(MainArr);
            Console.WriteLine($"Def time - {Milliseconds} ms");
            Console.WriteLine($"Def sum - {Sum(ArrRowsSum)}");

            SumWithThread();
            Console.WriteLine($"Threads time - {Milliseconds} ms");
            Console.WriteLine($"Threads sum -  {Sum(ArrRowsSum)}");

            SumWithTask();
            Console.WriteLine($"Task time - {Milliseconds} ms");
            Console.WriteLine($"Task sum -  {Sum(ArrRowsSum)}");

            Console.ReadKey();
        }

        private static void SumWithThread()
        {
            Clear(ArrRowsSum);

            var threadList = new List<Thread>(MaxRows);

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < MaxRows; i++)
            {
                threadList.Add(new Thread(Count));

                threadList[i].Start(i);
            }

            threadList.ForEach(th => th.Join());

            watch.Stop();

            Milliseconds = watch.ElapsedMilliseconds;
        }

        private static void SumWithTask()
        {
            Clear(ArrRowsSum);

            var tasks = new Task[MaxRows];

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => Count(i));

                tasks[i].Start();
            }

            Task.WaitAll();

            watch.Stop();

            Milliseconds = watch.ElapsedMilliseconds;
        }

        private static void SumDefault(int[][] arr)
        {
            Clear(ArrRowsSum);

            var watch = Stopwatch.StartNew();

            var resultArr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                Count(i);
            }

            watch.Stop();

            Milliseconds = watch.ElapsedMilliseconds;
        }

        private static void InitArray(int[][] arr)
        {
            var random = new Random();

            for (var i = 0; i < MaxRows; i++)
            {
                MainArr[i] = new int[MaxCols];

                for (var j = 0; j < MainArr[i].Length; j++)
                {
                    MainArr[i][j] = random.Next(-100, 100);
                }
            }
        }

        private static void InitArrayDefault(int[][] arr)
        {

            for (var i = 0; i < MaxRows; i++)
            {
                MainArr[i] = new int[MaxCols];

                for (var j = 0; j < MainArr[i].Length; j++)
                {
                    MainArr[i][j] = 1;
                }
            }
        }

        private static void Count(object obj)
        {
            var index = (int)obj;

            var num = MainArr[index].Sum();

            ArrRowsSum[index] = num;
        }

        private static void Clear(int[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }

        private static int Sum(int[] arr)
        {
            return arr.Sum();
        }

        private static void Display(int[] arr, string message)
        {
            Console.WriteLine(message);

            foreach (var el in arr)
            {
                Console.WriteLine($"{el}");
            }

            Console.WriteLine(new string('-', 50));
        }

        private static void Display(int[][] arr, string message)
        {
            Console.WriteLine(message);

            for (var i = 0; i < arr.Length; i++)
            {
                for (var j = 0; j < arr[i].Length; ++j)
                {
                    Console.Write($"{arr[i][j]}      ");
                }

                Console.WriteLine();
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
