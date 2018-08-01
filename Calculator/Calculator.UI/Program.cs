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
        private const int MaxRows = 5;
        private const int MaxCols = 4000000;

        private static void Main()
        {
            var arr = InitArray();

            var watchDef = Stopwatch.StartNew();
            Console.WriteLine($"Def sum - {SumDefault(arr)}");
            watchDef.Stop();
            Console.WriteLine($"Def time - {watchDef.ElapsedMilliseconds} ms");

            var watchThreads = Stopwatch.StartNew();
            Console.WriteLine($"Threads sum -  {SumWithThread(arr)}");
            watchThreads.Stop();
            Console.WriteLine($"Threads time - {watchThreads.ElapsedMilliseconds} ms");

            //!!!!
            var watchThreadPool = Stopwatch.StartNew();
            //SumWithThreadPool();
            Console.WriteLine($"ThreadPool sum -  {SumWithThreadPool(arr)}");
            watchThreadPool.Stop();
            Console.WriteLine($"ThreadPool time - {watchThreadPool.ElapsedMilliseconds} ms");

            var watchTasks = Stopwatch.StartNew();
            Console.WriteLine($"Task sum -  {SumWithTask(arr)}");
            watchTasks.Stop();
            Console.WriteLine($"Task time - {watchTasks.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }

        private static int SumWithThread(int[][] sentArray)
        {
            var threadList = new List<Thread>(MaxRows);
            var arr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                { arr[index] = sentArray[index].Sum(); }));

                threadList[i].Start();
            }

            threadList.ForEach(th => th.Join());

            return arr.Sum();
        }

        private static int SumWithThreadPool(int[][] arr)
        {
            var resultArr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                var index = i;

                //ThreadPool.QueueUserWorkItem((val) => { resultArr[index] = arr[index].Sum(); });
                ThreadPool.QueueUserWorkItem((val) => { resultArr[index] = arr[index].Sum(); });
            }

            return resultArr.Sum();
        }

        private static int SumWithTask(int[][] arr)
        {
            var tasks = new List<Task>();
            var resultArr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
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

            var resultArr = new int[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                resultArr[i] = arr[i].Sum();
            }

            return resultArr.Sum();
        }

        private static int[][] InitArray()
        {
            var random = new Random();
            var arr = new int[MaxRows][];

            for (var i = 0; i < MaxRows; i++)
            {
                arr[i] = new int[MaxCols];

                for (var j = 0; j < arr[i].Length; j++)
                {
                    arr[i][j] = random.Next(-100, 100);
                }
            }

            return arr;
        }
    }

    internal class ArrModel
    {
        int[] intputArr { get; set; }

        int[] outputArr { get; set; }
    }
}
