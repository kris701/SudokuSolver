using SudokuSolver.Models;

namespace SudokuSolver.Solvers.BacktrackSolvers.Pruners
{
    public interface IPruner
    {
        public bool Prune(SearchContext context);
    }
}
