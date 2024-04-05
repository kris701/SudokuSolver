namespace SudokuSolver.Tests.Models
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
