using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Calculator.UI
{
    public class ThreadParametr
    {
        public int Index { get; set; }

        public int[,] MainArr;
    }

    public class Program
    {
        const int MaxRows = 4;

        public static int[] Arr = new int[MaxRows];

        private static void Main()
        {
            var array = new[,]
            {
                {1, 2, 2,},
                {3, 3, 6,},
                {7, 6, 1,},
                {7, 6, 1,}
            };

            var threads = new Thread[MaxRows];

            for (var i = 0; i < MaxRows; i++)
            {
                Thread.Sleep(50);

                var parametrs = new ThreadParametr { Index = i,  MainArr = array };
                threads[i] = new Thread(new ParameterizedThreadStart(Count));

                threads[i].Start(parametrs);
            }

            //Console.WriteLine(array.GetRow(1));

            for (var i = 0; i < Arr.Length; i++)
            {
                Console.WriteLine(Arr[i]);
            }

            Console.ReadKey();
        }

        private static void Count(object obj)
        {
                var parametrs = (ThreadParametr)obj;

                if (Arr[parametrs.Index] != 0)
                {
                    throw new ArgumentException("Уже есть значение");
                }

                var num = parametrs.MainArr.GetRow(parametrs.Index);

                Arr[parametrs.Index] = num;
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
