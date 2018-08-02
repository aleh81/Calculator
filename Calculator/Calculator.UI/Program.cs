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
        private static int counter;
        private static object locker = new object();

        private static void Main()
        {
            var initArr = InitArray(500, 70000);

            Task1(initArr);

            Console.WriteLine(new string('-', 30));

            Task2(initArr);

            Console.ReadKey();
        }

        private static int SumWithParallel(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            Parallel.For(0, arr.Length, i => rowSumVector[i] = arr[i].Sum());

            return rowSumVector.Sum();
        }

        private static int SumFromSyncedThreadsWithParallel(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            Reset();

            Parallel.For(0, arr.Length, i =>
            {
                try
                {
                    Monitor.Enter(locker);

                    counter += arr[i].Sum();
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            });

            return counter;
        }

        private static int SumWithThread(int[][] arr)
        {
            var threadList = new List<Thread>(arr.Length);
            var rowSumVector = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                {
                    rowSumVector[index] = arr[index].Sum();
                }));

                threadList[i].Start();
            }

            threadList.ForEach(th => th.Join());

            return rowSumVector.Sum();
        }

        private static int SumFromSyncedTreads(int[][] arr)
        {
            var threadList = new List<Thread>(arr.Length);

            Reset();

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                {
                    lock (locker)
                    {
                        counter += arr[index].Sum();
                    }
                }));

                threadList[i].Start();

                threadList.ForEach(th => th.Join());
            }

            return counter;
        }

        private static int SumWithThreadPool(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            using (var cde = new CountdownEvent(arr.Length))
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    var index = i;

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        rowSumVector[index] = arr[index].Sum();

                        cde.Signal();
                    });
                }

                cde.Wait();
            }

            return rowSumVector.Sum();
        }

        private static int SumFromSyncedThreadPool(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            Reset();

            using (var cde = new CountdownEvent(arr.Length))
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    var index = i;

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        lock (locker)
                        {
                            counter += arr[index].Sum();
                        }

                        cde.Signal();
                    });
                }

                cde.Wait();
            }

            return counter;
        }

        private static int SumWithTask(int[][] arr)
        {
            var tasks = new List<Task>();
            var rowSumVector = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                var task = new Task(() =>
                {
                    rowSumVector[index] = arr[index].Sum();
                });

                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return rowSumVector.Sum();
        }

        private static int SumWithSyncedTask(int[][] arr)
        {
            var tasks = new List<Task>();

            Reset();

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                var task = new Task(() =>
                {
                    lock (locker)
                    {
                        counter += arr[index].Sum();
                    }
                });

                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return counter;
        }

        private static int SumDefault(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                rowSumVector[i] = arr[i].Sum();
            }

            return rowSumVector.Sum();
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

        private static void Reset()
        {
            counter = 0;
        }

        private static void Task1(int[][] initArr)
        {
            var watchDef = Stopwatch.StartNew();
            Console.WriteLine($"Def sum - {SumDefault(initArr)}");
            watchDef.Stop();
            Console.WriteLine($"Def time - {watchDef.ElapsedMilliseconds} ms");

            var watchThreads = Stopwatch.StartNew();
            Console.WriteLine($"Threads sum -  {SumWithThread(initArr)}");
            watchThreads.Stop();
            Console.WriteLine($"Threads time - {watchThreads.ElapsedMilliseconds} ms");

            var watchThreadPool = Stopwatch.StartNew();
            Console.WriteLine($"ThreadPool sum -  {SumWithThreadPool(initArr)}");
            watchThreadPool.Stop();
            Console.WriteLine($"ThreadPool time - {watchThreadPool.ElapsedMilliseconds} ms");

            var watchTasks = Stopwatch.StartNew();
            Console.WriteLine($"Task sum -  {SumWithTask(initArr)}");
            watchTasks.Stop();
            Console.WriteLine($"Task time - {watchTasks.ElapsedMilliseconds} ms");

            var watchParallel = Stopwatch.StartNew();
            Console.WriteLine($"Parallel sum - {SumWithParallel(initArr)}");
            watchParallel.Stop();
            Console.WriteLine($"Parallel time - {watchParallel.ElapsedMilliseconds}");
        }

        private static void Task2(int[][] initArr)
        {
            var watchThreads = Stopwatch.StartNew();
            Console.WriteLine($"Synced Threads sum - {SumFromSyncedTreads(initArr)}");
            watchThreads.Stop();
            Console.WriteLine($"Synced Threads time - {watchThreads.ElapsedMilliseconds}");

            var watchParallel = Stopwatch.StartNew();
            Console.WriteLine($"Synced Parallel sum - {SumFromSyncedThreadsWithParallel(initArr)}");
            watchParallel.Stop();
            Console.WriteLine($"Synced Parallel time - {watchParallel.ElapsedMilliseconds}");

            var watchThreadPool = Stopwatch.StartNew();
            Console.WriteLine($"Synced ThreadPool sum - {SumFromSyncedThreadPool(initArr)}");
            watchThreadPool.Stop();
            Console.WriteLine($"Synced ThreadPool time - {watchThreadPool.ElapsedMilliseconds}");

            var watchTask = Stopwatch.StartNew();
            Console.WriteLine($"Synced Task sum - {SumWithSyncedTask(initArr)}");
            watchTask.Stop();
            Console.WriteLine($"Synced Task time - {watchTask.ElapsedMilliseconds}");
        }
    }
}
