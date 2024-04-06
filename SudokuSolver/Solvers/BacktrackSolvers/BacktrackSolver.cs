using SudokuSolver.Models;
using System.Diagnostics;
using System.Timers;

namespace SudokuSolver.Solvers.BacktrackSolvers
{
    public class BacktrackSolver : ISolver
    {
        private static readonly Dictionary<string, SearchOptions> _configurations = new Dictionary<string, SearchOptions>()
        {
            { "Default", new SearchOptions() },
            { "NonOptimised", new SearchOptions(){
                GroundLegalCandidatesOnly = false,
                PruneCertains = false,
                PruneHiddenPairs = false,
                PruneNakedPairs = false
            } }
        };

        public int Calls { get; private set; }
        public int Invalids { get; private set; }
        public TimeSpan SearchTime { get; private set; }
        public TimeSpan Timeout { get; set; }
        public bool TimedOut { get; private set; }
        public SearchOptions Options { get; set; } = new SearchOptions();
        private string _configuration = "Default";
        public string Configuration {
            get { return _configuration; } 
            set {
                if (!_configurations.ContainsKey(value))
                    throw new BacktrackSolverException($"'{value}' is an invalid configuration for this solver! Options are: {string.Join(",", Configurations())}");
                Options = _configurations[value];
                _configuration = value;
            } 
        }

        private byte _size = 0;
        private Preprocessor _preprocessor;
        private Stopwatch? _watch;
        private bool _stop = false;
        private int _lastCalls = 0;

        public List<string> Configurations() => _configurations.Keys.ToList();

        public SudokuBoard? Solve(SudokuBoard board)
        {
            _stop = false;
            TimedOut = false;
            Calls = 0;
            Invalids = 0;
            _size = (byte)(board.BlockSize * board.BlockSize);
            _preprocessor = new Preprocessor(Options, _size);

            _watch = new Stopwatch();
            var timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Elapsed += (s, e) =>
            {
                if (Options.EnableLog)
                    Console.WriteLine("Timed out!");
                TimedOut = true;
                _stop = true;
            };
            timeoutTimer.AutoReset = false;
            if (Timeout.TotalMilliseconds > 0)
            {
                timeoutTimer.Interval = Timeout.TotalMilliseconds;
                timeoutTimer.Start();
            }

            var logTimer = new System.Timers.Timer();
            logTimer.Elapsed += LogUpdate;
            logTimer.AutoReset = true;
            logTimer.Interval = 1000;
            if (Options.EnableLog)
                logTimer.Start();
            _watch.Start();
            SudokuBoard? result = null;
            if (Options.EnableLog)
                Console.WriteLine($"Preprocessing...");
            var processed = _preprocessor.Preprocess(board);
            if (Options.EnableLog)
                Console.WriteLine($"Preprocess complete!");
            if (!_preprocessor.Failed)
            {
                if (Options.EnableLog)
                    Console.WriteLine($"Solving...");
                result = SolveInner(processed);
            }
            _watch.Stop();
            logTimer.Stop();
            timeoutTimer.Stop();
            SearchTime = _watch.Elapsed;
            if (Options.EnableLog)
                Console.WriteLine($"Took {Calls} calls and {SearchTime} time to solve.");
            return result;
        }

        private void LogUpdate(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"[t={Math.Round(_watch!.Elapsed.TotalSeconds, 0)}s] Calls: {Calls} [{Calls - _lastCalls}/s], Invalids {Invalids}");
            _lastCalls = Calls;
        }

        private SudokuBoard? SolveInner(SudokuBoard board, int bestOffset = 0)
        {
            if (_stop)
                return null;

            Calls++;

            var loc = GetBestCell(board, bestOffset);
            if (loc == null)
            {
                if (board.IsComplete())
                    return board;
                return null;
            }

            var possibilities = _preprocessor.Candidates[loc.X, loc.Y];
            for(int i = 0; i < possibilities.Count; i++)
            {
                if (possibilities[i].IsLegal(board))
                {
                    var copy = board.Copy();
                    possibilities[i].Apply(copy);
                    var result = SolveInner(copy, bestOffset + 1);
                    if (result != null)
                        return result;
                }
            }
            Invalids++;
            return null;
        }

        private CellPosition? GetBestCell(SudokuBoard board, int bestOffset)
        {
            var cardinalities = _preprocessor.Cardinalities;
            var count = cardinalities.Count;
            for (int i = bestOffset; i < count; i++)
                if (board[cardinalities[i].X, cardinalities[i].Y] == board.BlankNumber)
                    return cardinalities[i];
            return null;
        }
    }
}
