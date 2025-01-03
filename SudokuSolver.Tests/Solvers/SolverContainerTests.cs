﻿using CSVToolsSharp;
using SudokuSolver.Models;
using SudokuSolver.Solvers;
using System.Text;
using ToMarkdown;

namespace SudokuSolver.Tests.Solvers
{
    [TestClass]
    public class SolverContainerTests
    {
        public static List<SolverOptions> _solvers = new List<SolverOptions>() {
            SolverOptions.SequentialBacktrack,
            SolverOptions.LogicalWithSequentialBacktrack,
            SolverOptions.CardinalityBacktrack,
            SolverOptions.LogicalWithCardinalityBacktrack,
            SolverOptions.Logical,
            SolverOptions.RandomBacktrack,
            SolverOptions.LogicalWithRandomBacktrack
        };
        public static IEnumerable<object[]> Data() => BaseTests.TestCases(_solvers);

        private static readonly Dictionary<SolverOptions, List<int>> _calls = new Dictionary<SolverOptions, List<int>>();
        private static readonly Dictionary<SolverOptions, List<double>> _searchTimes = new Dictionary<SolverOptions, List<double>>();
        private static readonly Dictionary<SolverOptions, int> _solved = new Dictionary<SolverOptions, int>();
        private static readonly string _readmeFile = "../../../../README.md";
        private static readonly string _tempFile = "benchmarkRes.csv";
        private static string _autogenComment = "<!-- This section is auto generated. -->";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            foreach (SolverOptions solverOption in _solvers)
            {
                if (!_solved.ContainsKey(solverOption))
                    _solved.Add(solverOption, 0);
                if (!_searchTimes.ContainsKey(solverOption))
                    _searchTimes.Add(solverOption, new List<double>());
                if (!_calls.ContainsKey(solverOption))
                    _calls.Add(solverOption, new List<int>());
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data), DynamicDataSourceType.Method)]
        public void Can_Solve(string dataset, string boardStr, SolverOptions solverOption)
        {
            // ARRANGE
            var board = new SudokuBoard(boardStr);
            var solver = SolverBuilder.GetSolver(solverOption);
            solver.Timeout = BaseTests.Timeout;

            // ACT
            var result = solver.Solve(board);

            // ASSERT
            _calls[solverOption].Add(solver.Calls);
            if (solver.Stop)
                Assert.Inconclusive();

            if (result != null && result.IsComplete())
            {
                _solved[solverOption]++;
                _searchTimes[solverOption].Add(solver.SearchTime.TotalMilliseconds);
            }
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
                if (_calls[key].Count > 0)
                {
                    result.MaxCalls = _calls[key].Max();
                    result.MinCalls = _calls[key].Min();
                    result.AvgCalls = Math.Round(_calls[key].Average(), 2);
                }
                results.Add(result);
            }

            results = results.OrderByDescending(x => x.Solved).ThenBy(y => y.AvgTime).ThenByDescending(x => x.AvgCalls).ToList();

            var autogenText = $"Benchmark is run on {BaseTests.BenchmarkCount()} different Sudoku boards with a {BaseTests.Timeout.TotalSeconds}s time limit.{Environment.NewLine}";
            autogenText += "The results are ordered by solved instances, then by lowest average search time and finally by average calls.";
            autogenText += Environment.NewLine;
            var resultsText = results.ToMarkdownTable(new List<string>() {
                "Solver",
                "Solved".ToMarkdown(ToMarkdownExtensions.StringStyle.Bold),
                "Avg Search (ms)".ToMarkdown(ToMarkdownExtensions.StringStyle.Bold),
                "Avg Calls".ToMarkdown(ToMarkdownExtensions.StringStyle.Bold),
                "Max Search (ms)",
                "Min Search (ms)",
                "Max Calls",
                "Min Calls"});
            autogenText += Environment.NewLine + resultsText;
#if RELEASE
            var readme = File.ReadAllText(_readmeFile);
            var readmeStart = readme.Substring(0, readme.IndexOf(_autogenComment) + _autogenComment.Length) + Environment.NewLine;
            var readmeEnd = readme.Substring(readme.LastIndexOf(_autogenComment)) + Environment.NewLine;
            var sb = new StringBuilder();
            sb.AppendLine(readmeStart);
            sb.AppendLine(autogenText);
            sb.AppendLine(readmeEnd);

            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
            File.WriteAllText(_tempFile, CSVSerialiser.Serialise(results));
            File.WriteAllText(_readmeFile, sb.ToString());
#endif
        }

        private class ExperimentResults()
        {
            [CSVColumn("solver")]
            public string Solver { get; set; } = "";
            [CSVColumn("solved")]
            public int Solved { get; set; } = 0;
            [CSVColumn("avgtime")]
            public double AvgTime { get; set; } = -1;
            [CSVColumn("avgcalls")]
            public double AvgCalls { get; set; } = -1;

            [CSVColumn("maxtime")]
            public double MaxTime { get; set; } = -1;
            [CSVColumn("mintime")]
            public double MinTime { get; set; } = -1;

            [CSVColumn("maxcall")]
            public double MaxCalls { get; set; } = -1;
            [CSVColumn("mincalls")]
            public double MinCalls { get; set; } = -1;
        }
    }
}
