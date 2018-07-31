using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Calculator.UI
{
    public class Program
    {
        const int MaxRows = 4;
        const int MaxCols = 4;

        public static int[] Arr = new int[MaxRows];
        public static int[,] mainArr = new int[MaxRows, MaxCols];

        private static void Main()
        {
            InitArray(mainArr);

            var threads = new Thread[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                Thread.Sleep(50);

                threads[i] = new Thread(Count);
                threads[i].Start(i);
            }

            for (var i = 0; i < Arr.Length; i++)
            {
                Console.WriteLine(Arr[i]);
            }

            var sumDefault = SumDefault(mainArr);

            Console.WriteLine("Default Sum: ");

            for (var i = 0; i < sumDefault.Length; i++)
            {
                Console.WriteLine(sumDefault[i]);
            }

            Console.ReadKey();
        }

        private static MultiArrCount(int)
        {

        }

        private static void InitArray(int[,] arr)
        {
            var random = new Random();

            for (var i = 0; i < MaxRows; i++)
            {
                for (var j = 0; j < MaxCols; j++)
                {
                    arr[i, j] = random.Next(-100, 100);
                }
            }
        }

        private static int[] SumDefault(int[,] arr)
        {
            int[]resultArr = new int[MaxRows];

            for(var i = 0; i < MaxRows; i++)
            {
                resultArr[i] = arr.GetRow(i);
            }

            return resultArr;
        }

        private static void Count(object obj)
        {
            var index = (int)obj;

            if (Arr[index] != 0)
            {
                throw new ArgumentException("ERROR: Value already exsist");
            }

            var num = mainArr.GetRow(index);

            Arr[index] = num;
        }

        private static void Display(int[] arr, string message)
        {
            Console.WriteLine(message);

            for(var i = 0; i < arr.Length; i++)
            {
             
            }
        }
    }

    public static class ArrayExt
    {
        public static int GetRow(this int[,] array, int row)
        {
            if (array == null)
            {
                throw new ArgumentNullException("ERROR: Null Array");
            }

            var cols = array.GetUpperBound(1) + 1;
            int[] result = new int[cols];
            var size = Marshal.SizeOf<int>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result.Sum();
        }
    }
}
