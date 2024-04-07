using SudokuSolver.Models;
using SudokuSolver.Preprocessors;

namespace SudokuSolver.Solvers.GuidedBacktrackSolvers.Heuristics
{
    public interface IHeuristic
    {
        public int Value(SudokuBoard board, IPreprocessor preprocessor, CellAssignment assignment);
    }
}
