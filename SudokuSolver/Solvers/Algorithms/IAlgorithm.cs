using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms
{
    public interface IAlgorithm
    {
        public string Name { get; }
        public bool Stop { get; set; }
        public int Calls { get; }
        public SearchContext Solve(SearchContext context);
    }
}
