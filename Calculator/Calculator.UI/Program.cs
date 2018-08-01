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
        private const int MaxRows = 5;
        private const int MaxCols = 4000000;

        public static int[] ArrRowsSum = new int[MaxRows];
        public static int[][] MainArr = new int[MaxRows][];

        private static void Main()
        {
            InitArray(MainArr);

            var watchDef = Stopwatch.StartNew();
            SumDefault(MainArr);
            Console.WriteLine($"Def sum - {ArrRowsSum.Sum()}");
            watchDef.Stop();
            Console.WriteLine($"Def time - {watchDef.ElapsedMilliseconds} ms");

            var watchThreads = Stopwatch.StartNew();
            SumWithThread();
            Console.WriteLine($"Threads sum -  {ArrRowsSum.Sum()}");
            watchThreads.Stop();
            Console.WriteLine($"Threads time - {watchThreads.ElapsedMilliseconds} ms");

            var watchThreadPool = Stopwatch.StartNew();
            SumWithThreadPool();
            Console.WriteLine($"ThreadPool sum -  {ArrRowsSum.Sum()}");
            watchThreadPool.Stop();
            Console.WriteLine($"ThreadPool time - {watchThreadPool.ElapsedMilliseconds} ms");

            var watchTasks = Stopwatch.StartNew();
            SumWithTask();
            Console.WriteLine($"Task sum -  {ArrRowsSum.Sum()}");
            watchTasks.Stop();
            Console.WriteLine($"Task time - {watchTasks.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }

        private static void SumWithThread()
        {
            var threadList = new List<Thread>(MaxRows);

            for (var i = 0; i < MaxRows; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                { ArrRowsSum[index] = MainArr[index].Sum(); }));

                threadList[i].Start();
            }

            threadList.ForEach(th => th.Join());
        }

        private static void SumWithThreadPool()
        {
            for(var i = 0; i < MaxRows; i++)
            {
                var index = i;

                ThreadPool.QueueUserWorkItem(new WaitCallback(Count), index);
            }
        }

        private static void Count(object state)
        {
            int index = (int)state;

            ArrRowsSum[index] = MainArr[index].Sum();
        }

        private static void SumWithTask()
        {
            var tasks = new List<Task>();

            for (var i = 0; i < MaxRows; i++)
            {
                var index = i;

                var task = new Task(() =>
                {
                    ArrRowsSum[index] = MainArr[index].Sum();
                });

                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void SumDefault(int[][] arr)
        {

            var resultArr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                ArrRowsSum[i] = MainArr[i].Sum();
            }
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
    }
}
