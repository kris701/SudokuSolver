namespace SudokuSolver.Tests.Models
{
    public class Benchmark
    {
        public string File { get; set; }
        public byte BlockSize { get; set; }

        public Benchmark(string file, byte cellSize)
        {
            File = file;
            BlockSize = cellSize;
        }
    }
}
