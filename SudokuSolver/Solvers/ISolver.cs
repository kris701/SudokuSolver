using SudokuSolver.Models;

namespace SudokuSolver.Solvers
{
    public interface ISolver
    {
        public int Calls { get; }
        public TimeSpan SearchTime { get; }
        public bool TimedOut { get; }
        public TimeSpan Timeout { get; set; }
        public SudokuBoard? Solve(SudokuBoard from);
    }
}
