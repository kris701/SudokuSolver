using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SudokuToolsSharp.Helpers
{
    public static class ArrayExt
    {
        public static T[] GetRow<T>(this T[,] array, int row)
        {
            int size = array.GetLength(0);
            var result = new T[size];
            for (int x = 0; x < size; x++)
                result[x] = array[x, row];
            return result;
        }

        public static T[] GetColumn<T>(this T[,] array, int column)
        {
            //int size = array.GetLength(0);
            //var result = new T[size];
            //for (int y = 0; y < size; y++)
            //    result[y] = array[column, y];
            //return result;

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, column * cols * size, result, 0, cols * size);

            return result;
        }
    }
}
