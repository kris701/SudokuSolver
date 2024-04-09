[assembly: Parallelize(Workers = 12, Scope = ExecutionScope.MethodLevel)]

namespace SudokuSolver.Tests
{
    public static class BaseTests
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
        private static readonly List<string> _benchmarks = new List<string>()
        {
            "../../../../Benchmarks/11puzzles.txt",
            "../../../../Benchmarks/timan.txt",
            "../../../../Benchmarks/mypuzzles.txt",
            "../../../../Benchmarks/95puzzles.txt",
        };

        public static IEnumerable<object[]> TestCases()
        {
            foreach (var benchmark in _benchmarks)
            {
                foreach (var line in File.ReadAllLines(benchmark))
                {
                    var values = new List<byte>();
                    foreach (var c in line)
                        values.Add(byte.Parse($"{c}"));
                    yield return new object[] {
                        line,
                        values
                    };
                }
            }
        }
    }
}
