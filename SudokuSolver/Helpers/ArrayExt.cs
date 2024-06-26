﻿using System.Runtime.InteropServices;

namespace SudokuSolver.Helpers
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

        public static bool IsUnique<T>(this T[] array)
        {
            var set = new HashSet<T>();
            for (byte i = 0; i < array.Length; i++)
            {
                if (set.Contains(array[i]))
                    return false;
                set.Add(array[i]);
            }

            return true;
        }
    }
}
