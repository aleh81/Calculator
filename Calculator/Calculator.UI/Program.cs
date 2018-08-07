using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Calculator.UI
{
    public class Program
    {
        private static void Main()
        {
            var array = CreateAndInitArray(1000, 10000);

            TestingMultithreadedArrayCounting(array);

            Console.WriteLine(new string('-', 30));

            TestingMultithreadedArrayCountingWithUsingSynchronization(array);

            Console.ReadKey();
        }

        private static int SumWithParallel(int[][] arr)
        {
            var rowSumVector = new int[arr.Length];

            Parallel.For(0, arr.Length, index =>
            {
                var sum = SumVector(arr[index]);

                AddSumInVectorField(out rowSumVector[index], sum);
            });

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
                    var sum = SumVector(arr[i]);

                    Monitor.Enter(locker);

                    sumCounter += sum;
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
                    var sum = SumVector(arr[index]);

                    AddSumInVectorField(out rowSumVector[index], sum);
                }));

                threadList[i].Start();
            }

            threadList.ForEach(th => th.Join());

            return rowSumVector.Sum();
        }

        private static void CountPositiveNumbersWithThread(int[][] arr)
        {
            var threadList = new List<Thread>(arr.Length);
            var rowSumVector = new int[arr.Length];
            var exList = new List<Exception>();

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                {
                    try
                    {
                        if (arr[index].Sum() < 0)
                        {
                            throw new MultiThreadingException($"Error in Thread with code: {Thread.CurrentThread.GetHashCode()} number not positive");
                        }

                        var sum = SumVector(arr[index]);

                        AddSumInVectorField(out rowSumVector[index], sum);

                        //throw new ArgumentException("Test exception");
                    }
                    catch (Exception ex)
                    {
                        exList.Add(ex);
                    }
                }));

                threadList[index].Start();
            }

            threadList.ForEach(th => { th.Join(); });

            if (exList.Count > 0)
            {
                throw new AggregateException(
                    "MyExceptions", exList);
            }
        }

        private static int SumFromInterlocedSyncedTreads(int[][] arr)
        {
            var sumCounter = 0;
            var threadList = new List<Thread>(arr.Length);

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                {
                    var sum = SumVector(arr[index]);

                    Interlocked.Exchange(ref sumCounter, sumCounter + sum);
                }));

                threadList[i].Start();

                threadList.ForEach(th => th.Join());
            }

            return sumCounter;
        }

        private static int SumFromLocedSyncedTreads(int[][] arr)
        {
            var sumCounter = 0;
            var threadList = new List<Thread>(arr.Length);
            var locker = new object();
            var random = new Random();

            for (var i = 0; i < arr.Length; i++)
            {
                var index = i;

                threadList.Add(new Thread(() =>
                {
                    var sum = SumVector(arr[index]);

                    lock (locker)
                    {
                        sumCounter += sum;
                    }
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
                        var sum = SumVector(arr[index]);

                        AddSumInVectorField(out rowSumVector[index], sum);

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
                        var sum = SumVector(arr[index]);

                        lock (locker)
                        {
                            sumCounter += sum;
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
                    var sum = SumVector(arr[index]);

                    AddSumInVectorField(out rowSumVector[index], sum);
                });

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());

            return rowSumVector.Sum();
        }

        private static void CountPositiveNumbersWithTask(int[][] arr)
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
                        throw new MultiThreadingException($"Error - TaskId = {Task.CurrentId} Sum = {SumVector(arr[index])}");
                    }

                    var sum = SumVector(arr[index]);

                    AddSumInVectorField(out rowSumVector[index], sum);

                    //throw new ArgumentException($"Test Exception in TaskId = {Task.CurrentId}");
                });

                taskList.Add(task);
            }

            try
            {
                Task.WaitAll(taskList.ToArray());
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }
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
                    var sum = SumVector(arr[index]);

                    lock (locker)
                    {
                        sumCounter += sum;
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
                var sum = SumVector(arr[i]);

                AddSumInVectorField(out rowSumVector[i], sum);
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
                    arr[i][j] = random.Next(-100, 101);
                }
            }

            return arr;
        }

        private static void AddSumInVectorField(out int outputVectorCell, int addingSum)
        {
            outputVectorCell = addingSum;
        }

        private static int SumVector(int[] arr)
        {
            return arr.Sum();
        }

        public static void ReciveOtherThreadExceptions(string code, Exception ex)
        {
            if (!(ex is MultiThreadingException))
            {
                Console.WriteLine($"Code: {code}, Error: {ex.Message}");
            }
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

            var watchThreadPositive = Stopwatch.StartNew();
            Console.ForegroundColor = ConsoleColor.Blue;
            var countNegNumbersFromThread = 0;
            try
            {
                CountPositiveNumbersWithThread(initArr);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is MultiThreadingException)
                    {
                        countNegNumbersFromThread++;
                    }
                    else
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            watchThreadPositive.Stop();
            Console.WriteLine($"Thread positive numbers time - {watchThreadPositive.ElapsedMilliseconds}");
            Console.WriteLine($"Thread negative numbers count - {countNegNumbersFromThread}");
            Console.ResetColor();

            var watchThreadPool = Stopwatch.StartNew();
            Console.WriteLine($"ThreadPool sum -  {SumWithThreadPool(initArr)}");
            watchThreadPool.Stop();
            Console.WriteLine($"ThreadPool time - {watchThreadPool.ElapsedMilliseconds} ms");

            var watchTasks = Stopwatch.StartNew();
            Console.WriteLine($"Task sum -  {SumWithTask(initArr)}");
            watchTasks.Stop();
            Console.WriteLine($"Task time - {watchTasks.ElapsedMilliseconds} ms");

            var taskNegotiveNumbers = 0;
            var watchTaskPositive = Stopwatch.StartNew();
            Console.ForegroundColor = ConsoleColor.Blue;
            try
            {
                CountPositiveNumbersWithTask(initArr);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (!(e is MultiThreadingException))
                    {
                        Console.WriteLine(e.Message);
                    }
                    else
                    {
                        taskNegotiveNumbers++;
                    }
                }
            }
            watchTaskPositive.Stop();
            Console.WriteLine($"Task positive time - {watchTaskPositive.ElapsedMilliseconds}");
            Console.WriteLine($"Task negative numbers count - {taskNegotiveNumbers}");
            Console.ResetColor();

            var watchParallel = Stopwatch.StartNew();
            Console.WriteLine($"Parallel sum - {SumWithParallel(initArr)}");
            watchParallel.Stop();
            Console.WriteLine($"Parallel time - {watchParallel.ElapsedMilliseconds}");
        }

        private static void TestingMultithreadedArrayCountingWithUsingSynchronization(int[][] initArr)
        {
            var watchThreads = Stopwatch.StartNew();
            Console.WriteLine($"Interlocked Synced Threads sum - {SumFromInterlocedSyncedTreads(initArr)}");
            watchThreads.Stop();
            Console.WriteLine($"Interlocekd Synced Threads time - {watchThreads.ElapsedMilliseconds}");

            var watchThreadsWithSleep = Stopwatch.StartNew();
            Console.WriteLine($"locked Synced Threads sum - {SumFromLocedSyncedTreads(initArr)}");
            watchThreadsWithSleep.Stop();
            Console.WriteLine($"locked Synced Threads time - {watchThreadsWithSleep.ElapsedMilliseconds}");

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

    class MultiThreadingException : Exception
    {
        public MultiThreadingException(string message) : base(message)
        {
        }
    }
}
