using SudokuToolsSharp.Models;
using System;
using System.Diagnostics;
using System.Timers;

namespace SudokuToolsSharp.Solvers.BacktrackSolvers
{
    public class BacktrackSolver : ISolver
    {
        private int _lastCalls = 0;
        public int Calls { get; private set; }
        public int Invalids { get; private set; }
        public TimeSpan SearchTime { get; private set; }
        public TimeSpan Timeout { get; set; }
        public SearchOptions Options { get; set; } = new SearchOptions();

        private int _size = 0;
        private Preprocessor _preprocessor;
        private Stopwatch? _watch;
        private bool _stop = false;

        public BacktrackSolver(int boardSize)
        {
            _size = boardSize;
            _preprocessor = new Preprocessor(Options, boardSize);
        }

        public SudokuBoard? Solve(SudokuBoard board)
        {
            if (board.CellSize * board.CellSize != _size)
                throw new Exception("Board size did not match the input!");

            _watch = new Stopwatch();
            var timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Elapsed += (s, e) =>
            {
                if (Options.EnableLog)
                    Console.WriteLine("Timed out!");
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
            var processed = _preprocessor.Preprocess(board);
            if (!_preprocessor.Failed)
                result = SolveInner(processed);
            _watch.Stop();
            logTimer.Stop();
            timeoutTimer.Stop();
            SearchTime = _watch.Elapsed;
            return result;
        }

        private void LogUpdate(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"[t={Math.Round(_watch.Elapsed.TotalSeconds,0)}s] Calls: {Calls} [{Calls - _lastCalls}/s], Invalids {Invalids}");
            _lastCalls = Calls;
        }

        private SudokuBoard? SolveInner(SudokuBoard board, int bestOffset = 0)
        {
            if (_stop)
                return null;

            Calls++;

            var loc = GetBestLocation(board, bestOffset);
            if (loc == null && board.IsComplete())
                return board;

            foreach (var possible in _preprocessor.Candidates[loc.X, loc.Y])
            {
                if (possible.IsLegal(board))
                {
                    var copy = board.Copy();
                    possible.Apply(copy);
                    var result = SolveInner(copy, bestOffset + 1);
                    if (result != null)
                        return result;
                }
            }
            Invalids++;
            return null;
        }

        private Position? GetBestLocation(SudokuBoard board, int bestOffset)
        {
            for(int i = bestOffset; i < _preprocessor.Cardinalities.Count; i++)
                if (board[_preprocessor.Cardinalities[i].X, _preprocessor.Cardinalities[i].Y] == board.BlankNumber)
                    return _preprocessor.Cardinalities[i];
            return null; 
        }
    }
}
