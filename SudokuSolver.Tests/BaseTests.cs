using SudokuSolver.Tests.Models;

[assembly: Parallelize(Workers = 12, Scope = ExecutionScope.MethodLevel)]

namespace SudokuSolver.Tests
{
    public static class BaseTests
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
        private static readonly List<Benchmark> _benchmarks = new List<Benchmark>()
        {
            new Benchmark("../../../../Benchmarks/11puzzles.txt", 3),
            new Benchmark("../../../../Benchmarks/timan.txt", 3),
            new Benchmark("../../../../Benchmarks/mypuzzles.txt", 3),
            new Benchmark("../../../../Benchmarks/2x2.txt", 2),
            new Benchmark("../../../../Benchmarks/95puzzles.txt", 3),
        };

        public static IEnumerable<object[]> TestCases()
        {
            foreach (var benchmark in _benchmarks)
            {
                foreach (var line in File.ReadAllLines(benchmark.File))
                {
                    var values = new List<byte>();
                    foreach (var c in line)
                        values.Add(byte.Parse($"{c}"));
                    yield return new object[] {
                        values,
                        benchmark.BlockSize
                    };
                }
            }
        }
    }
}
