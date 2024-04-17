using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Helpers
{
    public static class HashSetHelper
    {
        public static void AddRange<T>(this HashSet<T> list, IList<T> other)
        {
            foreach(var item in other)
                list.Add(item);
        }

        public static void AddRange<T>(this HashSet<T> list, HashSet<T> other)
        {
            foreach (var item in other)
                list.Add(item);
        }
    }
}
