using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuToolsSharp.Models
{
    public class Benchmark
    {
        public string File { get; set; }
        public int CellSize { get; set; }

        public Benchmark(string file, int cellSize)
        {
            File = file;
            CellSize = cellSize;
        }
    }
}
