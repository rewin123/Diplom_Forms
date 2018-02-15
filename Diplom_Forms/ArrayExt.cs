using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom_Forms
{
    public static class ArrayExt
    {
        public static void ForEach<T>(this T[,] array, Action<T> action)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for(int x = 0;x < width;x++)
            {
                for(int y = 0;y < height;y++)
                {
                    action(array[x, y]);
                }
            }
        }

        public static void ForEach<T>(this T[,] array, Func<T,T> func)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x,y] = func(array[x, y]);
                }
            }
        }

        public static void ForEach<T>(this T[,] array, Func<T> func)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x, y] = func();
                }
            }
        }

        public static void ForEach<T1,T2>(this T1[,] array, T2[,] link, Action<T1,T2> action)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    action(array[x, y], link[x, y]);
                }
            }
        }

        public static void ForEach<T1, T2>(this T1[,] array, T2[,] link, Func<T1, T2, T1> func)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x,y] = func(array[x, y], link[x, y]);
                }
            }
        }

        public static R[,] Create<T,R>(this T[,] array, Func<T,R> func)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            R[,] result = new R[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = func(array[x, y]);
                }
            }
            return result;
        }
        public static R[,] Create<T1,T2, R>(this T1[,] array, T2[,] link, Func<T1, T2, R> func)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            R[,] result = new R[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = func(array[x, y], link[x,y]);
                }
            }
            return result;
        }
    }
}
