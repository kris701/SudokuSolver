[assembly: Parallelize(Workers = 12, Scope = ExecutionScope.MethodLevel)]

namespace SudokuSolver.Tests
{
    public static class BaseTests
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
        private static readonly List<string> _benchmarks = new List<string>()
        {
            "../../../../Benchmarks/dataset1/11puzzles.txt",
            "../../../../Benchmarks/dataset1/timan.txt",
            "../../../../Benchmarks/dataset1/mypuzzles.txt",
            "../../../../Benchmarks/dataset1/95puzzles.txt",
            "../../../../Benchmarks/dataset2/data.txt",
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
