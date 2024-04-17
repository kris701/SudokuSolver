using SudokuSolver.Models;

namespace SudokuSolver.Solvers.Algorithms.LogicSolvers.LogicPruners
{
    public interface IPruner
    {
        public int PrunedCandidates { get; set; }
        public bool Prune(SearchContext context);
    }
}
