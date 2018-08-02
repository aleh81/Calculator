using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calculator.UI
{
    public class Program
    {
        private static void Main()
        {
            var arr = InitArray(250, 500000);

            var watchDef = Stopwatch.StartNew();
            Console.WriteLine($"Def sum - {SumDefault(arr)}");
            watchDef.Stop();
            Console.WriteLine($"Def time - {watchDef.ElapsedMilliseconds} ms");

            var watchThreads = Stopwatch.StartNew();
            Console.WriteLine($"Threads sum -  {SumWithThread(arr)}");
            watchThreads.Stop();
            Console.WriteLine($"Threads time - {watchThreads.ElapsedMilliseconds} ms");

            var watchThreadPool = Stopwatch.StartNew();
            Console.WriteLine($"ThreadPool sum -  {SumWithThreadPool(arr)}");
            watchThreadPool.Stop();
            Console.WriteLine($"ThreadPool time - {watchThreadPool.ElapsedMilliseconds} ms");

            var watchTasks = Stopwatch.StartNew();
            Console.WriteLine($"Task sum -  {SumWithTask(arr)}");
            watchTasks.Stop();
            Console.WriteLine($"Task time - {watchTasks.ElapsedMilliseconds} ms");

            var watchParallel = Stopwatch.StartNew();
            Console.WriteLine($"Parallel sum - {SumWithParallel(arr)}");
            watchParallel.Stop();
            Console.WriteLine($"Parallel time - {watchParallel.ElapsedMilliseconds}");

            Console.ReadKey();
        }

        private static int SumWithParallel(int[][] arr)
        {
            var resultArr = new int[arr.Length];

            Parallel.For(0, arr.Length, i => resultArr[i] = arr[i].Sum());

            return resultArr.Sum();
        }

        private static int SumWithThread(int[][] arr)
        {
            var threadList = new List<Thread>(arr.Length);
            var resultArr = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                { resultArr[index] = arr[index].Sum(); }));

                threadList[i].Start();
            }

            threadList.ForEach(th => th.Join());

            return resultArr.Sum();
        }

        private static int SumWithThreadPool(int[][] arr)
        {
            var resultArr = new int[arr.Length];

            using (var cde = new CountdownEvent(arr.Length))
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    var index = i;

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        resultArr[index] = arr[index].Sum();
                        cde.Signal();
                    });
                }

                cde.Wait();
            }

            return resultArr.Sum();
        }

        private static int SumWithTask(int[][] arr)
        {
            var tasks = new List<Task>();
            var resultArr = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                var task = new Task(() =>
                {
                    resultArr[index] = arr[index].Sum();
                });

                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return resultArr.Sum();
        }

        private static int SumDefault(int[][] arr)
        {
            var resultArr = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                resultArr[i] = arr[i].Sum();
            }

            return resultArr.Sum();
        }

        private static int[][] InitArray(int rowSize, int colSize)
        {
            var random = new Random();
            var arr = new int[rowSize][];

            for (var i = 0; i < rowSize; i++)
            {
                arr[i] = new int[colSize];

                for (var j = 0; j < arr[i].Length; j++)
                {
                    arr[i][j] = random.Next(-100, 100);
                }
            }

            return arr;
        }
    }
}
