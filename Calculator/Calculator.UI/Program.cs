using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Calculator.UI
{
    public class Program
    {
        private const int MaxRows = 20;
        private const int MaxCols = 1000000;

        public static int[] ArrRowsSum = new int[MaxRows];
        public static int[][] MainArr = new int[MaxRows][];

        public static long Milliseconds { get; set; }

        private static void Main()
        {
            InitArray(MainArr);
            // InitArrayDefault(MainArr);

            // Display(MainArr,"Display array");

            SumWithThread();
            Console.WriteLine($"Time of execute: {Milliseconds} ms");
            Display(ArrRowsSum, "Sum of rows of MainArr after async operations with Threads:");

            var sumDefault = SumDefault(MainArr);
            Console.WriteLine($"Time of execute: {Milliseconds} ms");
            Display(sumDefault, "Sum of rows of MainArr after sync operation");

            SumWithTask();
            Console.WriteLine($"Time of execute: {Milliseconds} ms");
            Display(ArrRowsSum, "Sum of rows of MainArr after async operations with Task");

            Console.ReadKey();
        }

        private static void SumWithThread()
        {
            Clear(ArrRowsSum);

            var watch = Stopwatch.StartNew();

            var threads = new Thread[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                threads[i] = new Thread(Count);
                threads[i].Start(i);
            }

            watch.Stop();

            Milliseconds = watch.ElapsedMilliseconds;
        }

        private static void SumWithTask()
        {
            Clear(ArrRowsSum);

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < MaxRows; i++)
            {
                var task = Task.Run(() => Count(i));

                task.Wait();
            }

            watch.Stop();

            Milliseconds = watch.ElapsedMilliseconds;
        }

        private static int[] SumDefault(int[][] arr)
        {
            Clear(ArrRowsSum);

            var watch = Stopwatch.StartNew();

            var resultArr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                resultArr[i] = arr[i].Sum();
            }

            watch.Stop();

            Milliseconds = watch.ElapsedMilliseconds;

            return resultArr;
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
