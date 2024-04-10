#if DEBUG
[assembly: Parallelize(Workers = 12, Scope = ExecutionScope.MethodLevel)]
#endif

namespace SudokuSolver.Tests
{
    public static class BaseTests
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
        private static readonly List<string> _benchmarks = new List<string>()
        {
            "../../../../Benchmarks/David-Carmel/11puzzles.txt",
            "../../../../Benchmarks/David-Carmel/timan.txt",
            "../../../../Benchmarks/David-Carmel/mypuzzles.txt",
            "../../../../Benchmarks/David-Carmel/95puzzles.txt",
            "../../../../Benchmarks/3M-Sudoku/3m-sudoku.txt",
            "../../../../Benchmarks/Sudoku-Hard/sudoku-hard.txt",
        };

        public static IEnumerable<object[]> TestCases()
        {
            foreach (var benchmark in _benchmarks)
            {
                var file = new FileInfo(benchmark);
                var fileName = file.Name.Replace(file.Extension, "");
                foreach (var line in File.ReadAllLines(benchmark))
                {
                    var values = new List<byte>();
                    foreach (var c in line)
                        values.Add(byte.Parse($"{c}"));
                    yield return new object[] {
                        fileName,
                        line,
                        values
                    };
                }
            }
        }
    }
}
