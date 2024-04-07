using SudokuSolver.Models;
using SudokuSolver.Preprocessors;
using System.Diagnostics;
using System.Timers;

namespace SudokuSolver.Solvers
{
    public abstract class BaseSolver : ISolver
    {
        public int Calls { get; internal set; }
        public TimeSpan PreprocessTime { get; internal set; }
        public TimeSpan SearchTime { get; internal set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
        public bool TimedOut { get; internal set; }
        public IPreprocessor Preprocessor { get; internal set; }
        public string Configuration { get; set; } = "Default";
        public List<string> Configurations() => new List<string>();

        internal bool _stop = false;

        private int _lastCalls = 0;
        private Stopwatch? _watch;

        public BaseSolver(IPreprocessor preprocessor)
        {
            Preprocessor = preprocessor;
        }

        public SudokuBoard? Solve(SudokuBoard board)
        {
            _stop = false;
            TimedOut = false;
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

            _watch.Start();

            var preprocessed = Preprocessor.Preprocess(board);

            _watch.Stop();
            PreprocessTime = _watch.Elapsed;
            _watch.Reset();
            _watch.Start();

            var result = Run(preprocessed, Preprocessor);

            _watch.Stop();
            logTimer.Stop();
            timeoutTimer.Stop();

            SearchTime = _watch.Elapsed;
            Console.WriteLine($"Took {Calls} calls and {SearchTime} time to solve with {PreprocessTime} preprocessing time.");
            return result;
        }

        public abstract SudokuBoard? Run(SudokuBoard board, IPreprocessor preprocessor);

        private void OnTimeout(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timed out!");
            TimedOut = true;
            _stop = true;
        }

        private void LogUpdate(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"[t={Math.Round(_watch!.Elapsed.TotalSeconds, 0)}s] Calls: {Calls} [{Calls - _lastCalls}/s]");
            _lastCalls = Calls;
        }
    }
}
