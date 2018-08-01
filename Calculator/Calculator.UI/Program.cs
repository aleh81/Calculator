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
        private const int MaxRows = 200;
        private const int MaxCols = 4000000;

        public static int[] ArrRowsSum = new int[MaxRows];
        public static int[][] MainArr = new int[MaxRows][];

        public static long Milliseconds { get; set; }

        private static Semaphore sem = new Semaphore(3, 3);

        private static void Main()
        {
            InitArray(MainArr);

            SumWithThread();
            Console.WriteLine($"Time of execute: {Milliseconds} ms");
            Console.WriteLine($"Sum of rows of MainArr after async operations with Threads: \n {Sum(ArrRowsSum)}");
            //Display(ArrRowsSum, "Sum of rows of MainArr after async operations with Threads:");

            var sumDefault = SumDefault(MainArr);
            Console.WriteLine($"Time of execute: {Milliseconds} ms");
            // Display(sumDefault, "Sum of rows of MainArr after sync operation");
            Console.WriteLine($"Sum of rows of MainArr after sync operation: \n {Sum(sumDefault)}");

            SumWithTask();
            Console.WriteLine($"Time of execute: {Milliseconds} ms");
            //Display(ArrRowsSum, "Sum of rows of MainArr after async operations with Task");
            Console.WriteLine($"Sum of rows of MainArr after async operations with Task: \n {Sum(ArrRowsSum)}");

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
               
                Thread.Sleep(50);
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
