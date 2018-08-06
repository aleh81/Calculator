using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator.UI
{
    public class Program
    {
        private static void Main()
        {
            var array = CreateAndInitArray(10, 7000);

            TestingMultithreadedArrayCounting(array);

            Console.WriteLine(new string('-', 30));

            TestingMultithreadedArrayCountingWithUsingSynchronization(array);

            Console.ReadKey();
        }

        private static int SumWithParallel(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            Parallel.For(0, arr.Length, index =>
            AddSumInVectorField(out rowSumVector[index], arr[index].Sum()));

            return rowSumVector.Sum();
        }

        private static int SumFromSyncedThreadsWithParallel(int[][] arr)
        {
            var sumCounter = 0;
            var locker = new object();

            Parallel.For(0, arr.Length, i =>
            {
                try
                {
                    Monitor.Enter(locker);

                    sumCounter += arr[i].Sum();
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            });

            return sumCounter;
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
                    AddSumInVectorField(out rowSumVector[index], arr[index].Sum());
                }));

                threadList[i].Start();
            }

            threadList.ForEach(th => th.Join());

            return rowSumVector.Sum();
        }

        private static int SumFromSyncedTreads(int[][] arr)
        {
            var sumCounter = 0;
            var threadList = new List<Thread>(arr.Length);

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                {
                    Interlocked.Exchange(ref sumCounter, sumCounter + arr[index].Sum());
                }));

                threadList[i].Start();

                threadList.ForEach(th => th.Join());
            }

            return sumCounter;
        }

        private static int SumWithThreadPool(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            using (var countDownEvent = new CountdownEvent(arr.Length))
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    var index = i;

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        AddSumInVectorField(out rowSumVector[index], arr[index].Sum());

                        countDownEvent.Signal();
                    });
                }

                countDownEvent.Wait();
            }

            return rowSumVector.Sum();
        }

        private static int SumFromSyncedThreadPool(int[][] arr)
        {
            var sumCounter = 0;
            var locker = new object();

            using (var countDownEvent = new CountdownEvent(arr.Length))
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    var index = i;

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        lock (locker)
                        {
                            sumCounter += arr[index].Sum();
                        }

                        countDownEvent.Signal();
                    });
                }

                countDownEvent.Wait();
            }

            return sumCounter;
        }

        private static int SumWithTask(int[][] arr)
        {
            var taskList = new List<Task>();
            var rowSumVector = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                var task = Task.Run(() =>
                {
                    AddSumInVectorField(out rowSumVector[index], arr[index].Sum());
                });

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            return rowSumVector.Sum();
        }

        private static int SumPositiveNumbersWithTask(int[][] arr)
        {
            var taskList = new List<Task>();
            var rowSumVector = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                var task = Task.Run(() =>
                {
                    if (arr[index].Sum() < 0)
                    {
                        throw new CustomException($"Error - TaskId = {Task.CurrentId} Sum = {arr[index].Sum()}");
                    }

                    AddSumInVectorField(out rowSumVector[index], arr[index].Sum());
                });

                taskList.Add(task);
            }
            try
            {
                Task.WaitAll(taskList.ToArray());
            }
            catch (AggregateException ae)
            {
                //foreach (var e in ae.InnerExceptions)
                //{
                //    if(e is CustomException)
                //    {
                //        Console.ForegroundColor = ConsoleColor.Red;
                //        Console.WriteLine(e.Message);
                //        Console.ResetColor();
                //    }
                //    else
                //    {
                //        throw;
                //    }
                //}
            }

            return rowSumVector.Sum();
        }

        private static int SumWithSyncedTask(int[][] arr)
        {
            var sumCounter = 0;
            var taskList = new List<Task>();
            var locker = new object();

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                var task = Task.Run(() =>
                {
                    lock (locker)
                    {
                        sumCounter += arr[index].Sum();
                    }
                });

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            return sumCounter;
        }

        private static int SumDefault(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            for (var i = 0; i < arr.Length; i++)
            {
                AddSumInVectorField(out rowSumVector[i], arr[i].Sum());
            }

            return rowSumVector.Sum();
        }

        private static int[][] CreateAndInitArray(int rowSize, int colSize)
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

        private static void AddSumInVectorField(out int outputVectorCell, int addingSum)
        {
            outputVectorCell = addingSum;
        }

        private static void TestingMultithreadedArrayCounting(int[][] initArr)
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

            var watchTaskPositive = Stopwatch.StartNew();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Task positive sum - {SumPositiveNumbersWithTask(initArr)}");
            watchTaskPositive.Stop();
            Console.WriteLine($"Task positive time - {watchTaskPositive.ElapsedMilliseconds}");
            Console.ResetColor();


            var watchParallel = Stopwatch.StartNew();
            Console.WriteLine($"Parallel sum - {SumWithParallel(initArr)}");
            watchParallel.Stop();
            Console.WriteLine($"Parallel time - {watchParallel.ElapsedMilliseconds}");
        }

        private static void TestingMultithreadedArrayCountingWithUsingSynchronization(int[][] initArr)
        {
            var watchThreads = Stopwatch.StartNew();
            Console.WriteLine($"Synced Threads sum - {SumFromSyncedTreads(initArr)}");
            watchThreads.Stop();
            Console.WriteLine($"Synced Threads time - {watchThreads.ElapsedMilliseconds}");

            var watchParallel = Stopwatch.StartNew();
            Console.WriteLine($"Synced Parallel sum - { SumFromSyncedThreadsWithParallel(initArr)}");
            watchParallel.Stop();
            Console.WriteLine($"Synced Parallel time - {watchParallel.ElapsedMilliseconds}");

            var watchThreadPool = Stopwatch.StartNew();
            Console.WriteLine($"Synced ThreadPool sum - {SumFromSyncedThreadPool(initArr)}");
            watchThreadPool.Stop();
            Console.WriteLine($"Synced ThreadPool time - {watchThreadPool.ElapsedMilliseconds}");

            var watchTask = Stopwatch.StartNew();
            Console.WriteLine($"Synced Task sum - { SumWithSyncedTask(initArr)}");
            watchTask.Stop();
            Console.WriteLine($"Synced Task time - {watchTask.ElapsedMilliseconds}");
        }
    }

    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }
    }
}
