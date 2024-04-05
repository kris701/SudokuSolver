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
        public static T[] GetRow<T>(this T[] array, int row, int size)
        {
            var result = new T[size];
            var dataSize = Marshal.SizeOf<T>();
            Buffer.BlockCopy(array, row * size * dataSize, result, 0, size * dataSize);
            return result;
        }

        public static T[] GetColumn<T>(this T[] array, int column, int size)
        {
            var result = new T[size];
            for (int y = 0; y < size; y++)
                result[y] = array[y * size + column];
            return result;
        }
    }
}
