using SudokuSolver.Models;
using SudokuSolver.Solvers.Algorithms;
using SudokuSolver.Solvers.Preprocessors;
using System.Diagnostics;
using System.Timers;

namespace SudokuSolver.Solvers
{
    public class SolverContainer
    {
        public bool Stop { get; internal set; }
        public int Calls { get; internal set; }
        public TimeSpan SearchTime { get; internal set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
        public List<IAlgorithm> Algorithms { get; }

        private int _lastCalls = 0;
        private Stopwatch? _watch;

        public SolverContainer(List<IAlgorithm> algorithms)
        {
            Algorithms = algorithms;
        }

        public SudokuBoard? Solve(SudokuBoard board)
        {
            Stop = false;
            Calls = 0;

            _watch = new Stopwatch();
            var timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Elapsed += OnTimeout;
            timeoutTimer.AutoReset = false;
            if (Timeout > TimeSpan.Zero)
            {
                timeoutTimer.Interval = Timeout.TotalMilliseconds;
                timeoutTimer.Start();
            }

            var logTimer = new System.Timers.Timer();
            logTimer.Elapsed += LogUpdate;
            logTimer.AutoReset = true;
            logTimer.Interval = 1000;
            logTimer.Start();

            var context = Preprocessor.Preprocess(board);
            Console.WriteLine($"\tSolver has {Algorithms.Count} stage(s)");

            _watch.Start();

            int count = 1;
            foreach (var algorithm in Algorithms)
            {
                Console.WriteLine($"\tStage '{algorithm.Name}' started ({count++}/{Algorithms.Count})");
                context = algorithm.Solve(context);
                if (context.Board.IsComplete())
                    break;
            }

            _watch.Stop();
            logTimer.Stop();
            timeoutTimer.Stop();

            SearchTime = _watch.Elapsed;
            Calls = 0;
            foreach (var algorithm in Algorithms)
                Calls += algorithm.Calls;
            if (context.Board == null)
                return null;
            if (!context.Board.IsComplete())
                return null;
            Console.WriteLine($"\tTook {Calls} calls and {SearchTime} time to solve");
            return context.Board;
        }

        private void OnTimeout(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("\tTimed out!");
            Stop = true;
            foreach (var algorithm in Algorithms)
                algorithm.Stop = true;
        }

        private void LogUpdate(object? sender, ElapsedEventArgs e)
        {
            Calls = 0;
            foreach (var algorithm in Algorithms)
                Calls += algorithm.Calls;
            Console.WriteLine($"\t[t={Math.Round(_watch!.Elapsed.TotalSeconds, 0)}s] Calls: {Calls} [{Calls - _lastCalls}/s]");
            _lastCalls = Calls;
        }
    }
}
