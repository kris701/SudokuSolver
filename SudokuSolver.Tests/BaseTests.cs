using SudokuSolver.Solvers;

#if DEBUG
[assembly: Parallelize(Workers = 12, Scope = ExecutionScope.MethodLevel)]
#endif
#if RELEASE
[assembly: Parallelize(Workers = 2, Scope = ExecutionScope.MethodLevel)]
#endif

namespace SudokuSolver.Tests
{
    public static class BaseTests
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);
        private static readonly List<string> _benchmarks = new List<string>()
        {
            "../../../../Benchmarks/David-Carmel/11puzzles.txt",
            "../../../../Benchmarks/David-Carmel/timan.txt",
            "../../../../Benchmarks/David-Carmel/mypuzzles.txt",
            "../../../../Benchmarks/David-Carmel/95puzzles.txt",
            "../../../../Benchmarks/3M-Sudoku/3m-sudoku.txt",
            "../../../../Benchmarks/Sudoku-Hard/sudoku-hard.txt",
            "../../../../Benchmarks/Mike/mike.txt",
        };

        public static int BenchmarkCount()
        {
            var count = 0;
            foreach (var benchmark in _benchmarks)
                count += File.ReadAllLines(benchmark).Length;
            return count;
        }

        public static IEnumerable<object[]> TestCases(List<SolverOptions> solvers)
        {
            foreach (var benchmark in _benchmarks)
            {
                var file = new FileInfo(benchmark);
                var fileName = file.Name.Replace(file.Extension, "");
                foreach (var line in File.ReadAllLines(benchmark))
                {
                    foreach (SolverOptions solverOption in solvers)
                        yield return new object[] {
                            fileName,
                            line,
                            solverOption
                        };
                }
            }
        }
    }
}
