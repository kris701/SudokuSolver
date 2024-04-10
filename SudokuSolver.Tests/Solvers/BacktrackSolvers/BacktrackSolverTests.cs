using SudokuSolver.Models;
using SudokuSolver.Solvers.BacktrackSolvers;
using ToMarkdown;

namespace SudokuSolver.Tests.Solvers.BacktrackSolvers
{
    [TestClass]
    public class BacktrackSolverTests
    {
        public static IEnumerable<object[]> Data() => BaseTests.TestCases();

        private static List<double> _searchTimes = new List<double>();
        private static string _readmeFile = "../../../../README.md";
        private static string _readme = "";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _readme = File.ReadAllText(_readmeFile);
            _readme = _readme.Substring(0, _readme.IndexOf("# Performance") + "# Performance".Length);
        }

        [TestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Method)]
        public void Can_Solve(string dataset, string boardStr, List<byte> boardValues)
        {
            // ARRANGE
            var board = new SudokuBoard(boardValues.ToArray());
            var solver = new BacktrackSolver();
            solver.Timeout = BaseTests.Timeout;

            // ACT
            var result = solver.Solve(board);

            // ASSERT
            if (solver.TimedOut)
                Assert.Inconclusive();
            else
                Assert.IsNotNull(result);
            _searchTimes.Add(solver.SearchTime.TotalMilliseconds);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var result = new ExperimentResults();
            result.Solved = _searchTimes.Count;
            result.MaxTime = Math.Round(_searchTimes.Max(), 2);
            result.MinTime = Math.Round(_searchTimes.Min(),2);
            result.AvgTime = Math.Round(_searchTimes.Average(),2);
            var text = new List<ExperimentResults>() { result }.ToMarkdownTable(new List<string>() { 
                "Sudokus Solved",
                "Max Search Time (ms)",
                "Min Search Time (ms)",
                "Average Search Time (ms)"});

            _readme += Environment.NewLine + text;
#if RELEASE
            File.WriteAllText(_readmeFile, _readme);
#endif
        }

        private class ExperimentResults()
        {
            public int Solved { get; set; }
            public double MaxTime { get; set; }
            public double MinTime { get; set; }
            public double AvgTime { get; set; }
        }
    }
}
