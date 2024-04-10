﻿using CSVToolsSharp;
using SudokuSolver.Models;
using SudokuSolver.Solvers;
using ToMarkdown;

namespace SudokuSolver.Tests.Solvers
{
    [TestClass]
    public class SolverContainerTests
    {
        public static List<SolverOptions> _solvers = new List<SolverOptions>() { 
            SolverOptions.BruteForceBacktrack
        };
        public static IEnumerable<object[]> Data() => BaseTests.TestCases(_solvers);

        private static readonly Dictionary<SolverOptions, List<double>> _searchTimes = new Dictionary<SolverOptions, List<double>>();
        private static readonly Dictionary<SolverOptions, int> _solved = new Dictionary<SolverOptions, int>();
        private static readonly string _readmeFile = "../../../../README.md";
        private static readonly string _tempFile = "benchmarkRes.csv";
        private static string _readme = "";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _readme = File.ReadAllText(_readmeFile);
            _readme = _readme.Substring(0, _readme.IndexOf("# Performance") + "# Performance".Length) + Environment.NewLine;
            _readme += $"Benchmark is run on {BaseTests.BenchmarkCount()} different Sudoku boards with a {BaseTests.Timeout.TotalSeconds}s time limit.{Environment.NewLine}";
            _readme += Environment.NewLine;
            foreach (SolverOptions solverOption in _solvers)
            {
                if (!_solved.ContainsKey(solverOption))
                    _solved.Add(solverOption, 0);
                if (!_searchTimes.ContainsKey(solverOption))
                    _searchTimes.Add(solverOption, new List<double>() { 0 });
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Method)]
        public void Can_Solve(string dataset, string boardStr, SolverOptions solverOption, List<byte> boardValues)
        {
            // ARRANGE
            var board = new SudokuBoard(boardValues.ToArray());
            var solver = SolverBuilder.GetSolver(solverOption);
            solver.Timeout = BaseTests.Timeout;

            // ACT
            var result = solver.Solve(board);

            // ASSERT
            if (!solver.Stop && result != null && result.IsComplete())
            {
                _solved[solverOption]++;
                _searchTimes[solverOption].Add(solver.SearchTime.TotalMilliseconds);
            }
            else
                Assert.Inconclusive();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var results = new List<ExperimentResults>();
            if (File.Exists(_tempFile))
            {
                var parsed = CSVSerialiser.Deserialise<ExperimentResults>(File.ReadAllText(_tempFile));
                foreach (var line in parsed)
                    if (!_searchTimes.Keys.ToList().Any(x => Enum.GetName(typeof(SolverOptions), x) == line.Solver))
                        results.Add(line);
            }

            foreach (var key in _searchTimes.Keys)
            {
                var result = new ExperimentResults();
                result.Solver = Enum.GetName(typeof(SolverOptions), key)!;
                result.Solved = _solved[key];

                if (_searchTimes[key].Count > 0)
                {
                    result.MaxTime = Math.Round(_searchTimes[key].Max(), 2);
                    result.MinTime = Math.Round(_searchTimes[key].Min(), 2);
                    result.AvgTime = Math.Round(_searchTimes[key].Average(), 2);
                }
                results.Add(result);
            }

            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
            File.WriteAllText(_tempFile, CSVSerialiser.Serialise(results));

            var text = results.ToMarkdownTable(new List<string>() {
                "Solver",
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
            [CSVColumn("solver")]
            public string Solver { get; set; } = "";
            [CSVColumn("solved")]
            public int Solved { get; set; } = 0;
            [CSVColumn("maxtime")]
            public double MaxTime { get; set; } = -1;
            [CSVColumn("mintime")]
            public double MinTime { get; set; } = -1;
            [CSVColumn("avgtime")]
            public double AvgTime { get; set; } = -1;
        }
    }
}
