using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public interface IPruner
    {
        public bool Prune(SearchContext context);
    }
}
