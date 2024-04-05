using SudokuToolsSharp.Models;

namespace SudokuToolsSharp.Solvers
{
    public interface ISolver
    {
        public int Calls { get; }
        public TimeSpan SearchTime { get; }
        public TimeSpan Timeout { get; set; }
        public SudokuBoard? Solve(SudokuBoard from);
    }
}
